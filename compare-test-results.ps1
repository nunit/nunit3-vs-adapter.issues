#!/usr/bin/env pwsh
# Compare test-passes.json with test-passes.baseline.json
# Compare test-fails.json with test-fails.baseline.json

$ErrorActionPreference = "Stop"

function Compare-TestResults {
    param(
        [string]$CurrentFile,
        [string]$BaselineFile,
        [string]$Label
    )
    
    Write-Host "`n=== Comparing $Label ===" -ForegroundColor Cyan
    
    if (-not (Test-Path $CurrentFile)) {
        Write-Host "ERROR: $CurrentFile not found" -ForegroundColor Red
        return
    }
    
    if (-not (Test-Path $BaselineFile)) {
        Write-Host "ERROR: $BaselineFile not found" -ForegroundColor Red
        return
    }
    
    $current = Get-Content $CurrentFile | ConvertFrom-Json
    $baseline = Get-Content $BaselineFile | ConvertFrom-Json
    
    # Create lookup dictionaries by issue+project (ignoring timestamps)
    $currentDict = @{}
    foreach ($entry in $current.test_results) {
        $key = "$($entry.issue)|$($entry.project)"
        $currentDict[$key] = $entry
    }
    
    $baselineDict = @{}
    foreach ($entry in $baseline.test_results) {
        $key = "$($entry.issue)|$($entry.project)"
        $baselineDict[$key] = $entry
    }
    
    # Find entries only in current
    $onlyInCurrent = @()
    foreach ($key in $currentDict.Keys) {
        if (-not $baselineDict.ContainsKey($key)) {
            $onlyInCurrent += $currentDict[$key]
        }
    }
    
    # Find entries only in baseline
    $onlyInBaseline = @()
    foreach ($key in $baselineDict.Keys) {
        if (-not $currentDict.ContainsKey($key)) {
            $onlyInBaseline += $baselineDict[$key]
        }
    }
    
    # Report results
    Write-Host "`nCurrent file: $($current.test_results.Count) entries" -ForegroundColor Yellow
    Write-Host "Baseline file: $($baseline.test_results.Count) entries" -ForegroundColor Yellow
    
    if ($onlyInCurrent.Count -gt 0) {
        Write-Host "`nEntries ONLY in current file ($($onlyInCurrent.Count)):" -ForegroundColor Green
        foreach ($entry in $onlyInCurrent) {
            Write-Host "  - $($entry.issue) / $($entry.project)" -ForegroundColor Green
        }
    }
    
    if ($onlyInBaseline.Count -gt 0) {
        Write-Host "`nEntries ONLY in baseline file ($($onlyInBaseline.Count)):" -ForegroundColor Red
        foreach ($entry in $onlyInBaseline) {
            Write-Host "  - $($entry.issue) / $($entry.project)" -ForegroundColor Red
        }
    }
    
    if ($onlyInCurrent.Count -eq 0 -and $onlyInBaseline.Count -eq 0) {
        Write-Host "`nNo differences found - all entries match" -ForegroundColor Green
    }
}

# Compare passes
Compare-TestResults -CurrentFile "test-passes.json" -BaselineFile "test-passes.baseline.json" -Label "test-passes"

# Compare fails
Compare-TestResults -CurrentFile "test-fails.json" -BaselineFile "test-fails.baseline.json" -Label "test-fails"

Write-Host "`n=== Comparison Complete ===" -ForegroundColor Cyan

