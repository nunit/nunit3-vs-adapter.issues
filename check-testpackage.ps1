# Script to check dump files for TestPackage element and count DLLs
$results = @()
$multipleDlls = @()

# Find all E*.dump files in Dump folders under bin directories
Get-ChildItem -Path . -Recurse -Filter "E*.dump" -ErrorAction SilentlyContinue | ForEach-Object {
    $file = $_
    
    try {
        # Read the XML file
        $content = Get-Content $file.FullName -Raw -ErrorAction Stop
        
        # Extract TestPackage content
        $testPackage = $null
        $dllCount = 0
        $dlls = @()
        
        # Try to match TestPackage element - format is <TestPackage>: dllname or <TestPackage>dllname</TestPackage>
        # Use regex with Singleline option to match across newlines
        $testPackageMatch = [regex]::Match($content, '<TestPackage>(.*?)</TestPackage>', [System.Text.RegularExpressions.RegexOptions]::Singleline)
        
        if ($testPackageMatch.Success) {
            $testPackageContent = $testPackageMatch.Groups[1].Value
            
            # Clean up the content - remove leading/trailing whitespace and newlines
            $testPackageContent = $testPackageContent.Trim()
            
            # Remove the colon and any leading whitespace after it (format: ": dllname")
            $testPackageContent = $testPackageContent -replace '^:\s*', ''
            
            # Extract all .dll references - match any text ending in .dll
            # This will catch dll names even if they're on separate lines
            $dllMatches = [regex]::Matches($testPackageContent, '([a-zA-Z0-9_\-\.]+\.dll)', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            
            if ($dllMatches.Count -gt 0) {
                $dlls = $dllMatches | ForEach-Object { $_.Value.Trim() } | Where-Object { $_ -ne '' }
            }
            
            $dllCount = $dlls.Count
            $testPackage = $testPackageContent
        }
        
        $result = [PSCustomObject]@{
            File = $file.FullName
            TestPackage = $testPackage
            DllCount = $dllCount
            Dlls = ($dlls -join ', ')
        }
        
        $results += $result
        
        # Check if more than one DLL
        if ($dllCount -gt 1) {
            $multipleDlls += $result
        }
        
    } catch {
        Write-Warning "Error reading $($file.FullName): $_"
    }
}

# Display results
Write-Host "`n=== Summary ===" -ForegroundColor Green
Write-Host "Total dump files checked: $($results.Count)" -ForegroundColor Cyan

Write-Host "`n=== TestPackage Analysis ===" -ForegroundColor Green
$singleDllCount = ($results | Where-Object { $_.DllCount -eq 1 }).Count
$multipleDllCount = ($results | Where-Object { $_.DllCount -gt 1 }).Count
$noDllCount = ($results | Where-Object { $_.DllCount -eq 0 -or $null -eq $_.TestPackage }).Count

Write-Host "Files with 1 DLL: $singleDllCount" -ForegroundColor Yellow
Write-Host "Files with multiple DLLs: $multipleDllCount" -ForegroundColor $(if ($multipleDllCount -gt 0) { "Red" } else { "Green" })
Write-Host "Files with no DLL found: $noDllCount" -ForegroundColor $(if ($noDllCount -gt 0) { "Yellow" } else { "Green" })

if ($multipleDlls.Count -gt 0) {
    Write-Host "`n=== Files with Multiple DLLs ===" -ForegroundColor Red
    $multipleDlls | ForEach-Object {
        Write-Host "`nFile: $($_.File)" -ForegroundColor Cyan
        Write-Host "  TestPackage: $($_.TestPackage)" -ForegroundColor Yellow
        Write-Host "  DLL Count: $($_.DllCount)" -ForegroundColor Yellow
        Write-Host "  DLLs: $($_.Dlls)" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n✓ No files found with multiple DLLs in TestPackage" -ForegroundColor Green
}

$noDllFiles = $results | Where-Object { $_.DllCount -eq 0 -or $null -eq $_.TestPackage }
if ($noDllFiles.Count -gt 0) {
    Write-Host "`n=== Files with No DLL Found ===" -ForegroundColor Yellow
    $noDllFiles | ForEach-Object {
        Write-Host "`nFile: $($_.File)" -ForegroundColor Cyan
        Write-Host "  TestPackage: '$($_.TestPackage)'" -ForegroundColor Yellow
        Write-Host "  DLL Count: $($_.DllCount)" -ForegroundColor Yellow
    }
}

# Show distribution
Write-Host "`n=== DLL Count Distribution ===" -ForegroundColor Green
$results | Group-Object DllCount | Sort-Object Name | ForEach-Object {
    Write-Host "  $($_.Name) DLL(s): $($_.Count) files" -ForegroundColor Yellow
}

# Show a few examples of TestPackage content
Write-Host "`n=== Sample TestPackage Contents ===" -ForegroundColor Green
$results | Select-Object -First 5 | ForEach-Object {
    Write-Host "`nFile: $($_.File)" -ForegroundColor Cyan
    Write-Host "  TestPackage: $($_.TestPackage)" -ForegroundColor Yellow
    Write-Host "  DLL Count: $($_.DllCount)" -ForegroundColor Yellow
}

