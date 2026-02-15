# Script to find all dump files and count RunningBy values
$dumpFiles = @()
$runningByCounts = @{}

# Find all E*.dump files in Dump folders under bin directories
Get-ChildItem -Path . -Recurse -Filter "E*.dump" -ErrorAction SilentlyContinue | ForEach-Object {
    $file = $_
    
    try {
        # Read the XML file
        $content = Get-Content $file.FullName -Raw -ErrorAction Stop
        
        # Extract RunningBy value using regex
        if ($content -match '<RunningBy>(.*?)</RunningBy>') {
            $runningByValue = $matches[1].Trim()
            $dumpFiles += [PSCustomObject]@{
                File = $file.FullName
                RunningBy = $runningByValue
            }
            
            # Count occurrences
            if ($runningByCounts.ContainsKey($runningByValue)) {
                $runningByCounts[$runningByValue]++
            } else {
                $runningByCounts[$runningByValue] = 1
            }
        } else {
            Write-Warning "No RunningBy element found in: $($file.FullName)"
        }
    } catch {
        Write-Warning "Error reading $($file.FullName): $_"
    }
}

# Display results
Write-Host "`n=== Summary ===" -ForegroundColor Green
Write-Host "Total dump files found: $($dumpFiles.Count)" -ForegroundColor Cyan

Write-Host "`n=== RunningBy Value Counts ===" -ForegroundColor Green
$runningByCounts.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object {
    Write-Host "$($_.Key): $($_.Value) files" -ForegroundColor Yellow
}

Write-Host "`n=== Files by RunningBy Value ===" -ForegroundColor Green
$dumpFiles | Group-Object RunningBy | Sort-Object Count -Descending | ForEach-Object {
    Write-Host "`n$($_.Name) ($($_.Count) files):" -ForegroundColor Cyan
    $_.Group | ForEach-Object {
        Write-Host "  - $($_.File)"
    }
}

