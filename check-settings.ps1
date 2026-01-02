# Script to check dump files for SynchronousEvents, ProcessModel, and DomainUsage settings
$results = @()
$synchronousEventsIssues = @()
$processModelIssues = @()
$domainUsageIssues = @()

# Expected values
$expectedSynchronousEvents = "False"
$expectedProcessModel = "InProcess"
$expectedDomainUsage = "Single"

# Find all E*.dump files in Dump folders under bin directories
Get-ChildItem -Path . -Recurse -Filter "E*.dump" -ErrorAction SilentlyContinue | ForEach-Object {
    $file = $_
    
    try {
        # Read the XML file
        $content = Get-Content $file.FullName -Raw -ErrorAction Stop
        
        # Extract settings using regex
        $synchronousEvents = $null
        $processModel = $null
        $domainUsage = $null
        
        if ($content -match "<setting name='SynchronousEvents' value='(.*?)'") {
            $synchronousEvents = $matches[1]
        }
        
        if ($content -match "<setting name='ProcessModel' value='(.*?)'") {
            $processModel = $matches[1]
        }
        
        if ($content -match "<setting name='DomainUsage' value='(.*?)'") {
            $domainUsage = $matches[1]
        }
        
        $result = [PSCustomObject]@{
            File = $file.FullName
            SynchronousEvents = $synchronousEvents
            ProcessModel = $processModel
            DomainUsage = $domainUsage
        }
        
        $results += $result
        
        # Check for issues
        if ($synchronousEvents -and $synchronousEvents -ne $expectedSynchronousEvents) {
            $synchronousEventsIssues += $result
        }
        
        if ($processModel -and $processModel -ne $expectedProcessModel) {
            $processModelIssues += $result
        }
        
        if ($domainUsage -and $domainUsage -ne $expectedDomainUsage) {
            $domainUsageIssues += $result
        }
        
    } catch {
        Write-Warning "Error reading $($file.FullName): $_"
    }
}

# Display results
Write-Host "`n=== Summary ===" -ForegroundColor Green
Write-Host "Total dump files checked: $($results.Count)" -ForegroundColor Cyan

Write-Host "`n=== SynchronousEvents ===" -ForegroundColor Green
Write-Host "Expected: $expectedSynchronousEvents" -ForegroundColor Yellow
if ($synchronousEventsIssues.Count -eq 0) {
    Write-Host "✓ All files have the expected value" -ForegroundColor Green
} else {
    Write-Host "✗ Found $($synchronousEventsIssues.Count) files with unexpected values:" -ForegroundColor Red
    $synchronousEventsIssues | ForEach-Object {
        Write-Host "  - $($_.File): $($_.SynchronousEvents)" -ForegroundColor Red
    }
}

Write-Host "`n=== ProcessModel ===" -ForegroundColor Green
Write-Host "Expected: $expectedProcessModel" -ForegroundColor Yellow
if ($processModelIssues.Count -eq 0) {
    Write-Host "✓ All files have the expected value" -ForegroundColor Green
} else {
    Write-Host "✗ Found $($processModelIssues.Count) files with unexpected values:" -ForegroundColor Red
    $processModelIssues | ForEach-Object {
        Write-Host "  - $($_.File): $($_.ProcessModel)" -ForegroundColor Red
    }
}

Write-Host "`n=== DomainUsage ===" -ForegroundColor Green
Write-Host "Expected: $expectedDomainUsage" -ForegroundColor Yellow
if ($domainUsageIssues.Count -eq 0) {
    Write-Host "✓ All files have the expected value" -ForegroundColor Green
} else {
    Write-Host "✗ Found $($domainUsageIssues.Count) files with unexpected values:" -ForegroundColor Red
    $domainUsageIssues | ForEach-Object {
        Write-Host "  - $($_.File): $($_.DomainUsage)" -ForegroundColor Red
    }
}

# Show value distribution
Write-Host "`n=== Value Distribution ===" -ForegroundColor Green

Write-Host "`nSynchronousEvents values:" -ForegroundColor Cyan
$results | Where-Object { $_.SynchronousEvents } | Group-Object SynchronousEvents | Sort-Object Count -Descending | ForEach-Object {
    Write-Host "  $($_.Name): $($_.Count) files" -ForegroundColor Yellow
}

Write-Host "`nProcessModel values:" -ForegroundColor Cyan
$results | Where-Object { $_.ProcessModel } | Group-Object ProcessModel | Sort-Object Count -Descending | ForEach-Object {
    Write-Host "  $($_.Name): $($_.Count) files" -ForegroundColor Yellow
}

Write-Host "`nDomainUsage values:" -ForegroundColor Cyan
$results | Where-Object { $_.DomainUsage } | Group-Object DomainUsage | Sort-Object Count -Descending | ForEach-Object {
    Write-Host "  $($_.Name): $($_.Count) files" -ForegroundColor Yellow
}

