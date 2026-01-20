# Plan: Use results.json as Single Source of Truth

## Summary

This plan simplifies test result storage by using **only `results.json` and `results-baseline.json`** as the single source of truth, eliminating the need for separate `test-passes.json` and `test-fails.json` files.

### Key Changes:
- **`results.json`**: Now incrementally updated (merged) instead of overwritten, contains all test results
- **`results-baseline.json`**: Replaces `test-passes.baseline.json` and `test-fails.baseline.json`
- **Removed files**: `test-passes.json`, `test-fails.json`, and their baseline counterparts are no longer used
- **Domain model filtering**: All pass/fail queries now use LINQ on `IssueResult` objects from `results.json`

### Benefits:
- Simpler storage (one file instead of four)
- No duplication of data
- Historical results preserved across runs
- More flexible querying using domain model
- Easier to maintain and understand

## Problem Statement

Currently, test results are stored in multiple files:
- `results.json` - completely overwritten on each test run, contains only results from the most recent run
- `test-passes.json` - summary of passing tests
- `test-fails.json` - summary of failing tests
- `test-passes.baseline.json` - baseline of passing tests
- `test-fails.baseline.json` - baseline of failing tests

This creates several problems:
- If you run a subset of issues, results for other issues are lost from `results.json`
- The Issue List View cannot show test results for issues that weren't in the most recent run
- Historical test result data is not preserved
- Duplicate storage of the same information in multiple formats
- Complex logic to keep multiple files in sync

## Solution Overview

**Simplify to use only `results.json` and `results-baseline.json`**:
1. **Make `results.json` the single source of truth**: Incrementally update it (merge new results with existing)
2. **Replace baseline files**: Use `results-baseline.json` instead of `test-passes.baseline.json` and `test-fails.baseline.json`
3. **Remove `test-passes.json` and `test-fails.json`**: No longer needed - all information is in `results.json`
4. **Update all consumers**: Update all code that reads from `test-passes.json`/`test-fails.json` to read from `results.json` instead
5. **Use domain model to filter**: Use `IssueResult` objects from `results.json` to filter by pass/fail status as needed

This simplifies storage, reduces duplication, and makes the domain model the source of truth for all test result queries.

## Implementation Tasks

### Task 1: Update SaveResultsAsync to Merge Instead of Overwrite

**File**: `Tools/IssueRunner.Core/Commands/RunTestsCommand.cs`

**Current Implementation**:
```csharp
private async Task SaveResultsAsync(
    string repositoryRoot,
    List<IssueResult> results,
    CancellationToken cancellationToken)
{
    var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
    var resultsPath = Path.Combine(dataDir, "results.json");
    
    var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    
    var json = JsonSerializer.Serialize(results, options);
    await File.WriteAllTextAsync(resultsPath, json, cancellationToken);
}
```

**Changes Required**:
1. Load existing `results.json` if it exists using `LoadPreviousResultsAsync` (already exists)
2. Create a dictionary keyed by `Number|ProjectPath` from existing results
3. For each new result, update the dictionary entry (overwrite existing entry for same issue+project combination)
4. Convert dictionary back to list and write merged results

**Key Considerations**:
- Use `Number` (int) and `ProjectPath` (string) as the composite key
- New results should overwrite existing entries for the same issue+project
- Preserve all existing entries that are not being updated
- Handle the case where `results.json` doesn't exist (first run)

**Implementation Steps**:
1. Call `LoadPreviousResultsAsync` to get existing results
2. Create `Dictionary<string, IssueResult>` keyed by `$"{result.Number}|{result.ProjectPath}"`
3. Add/update entries from new results
4. Convert dictionary values back to list
5. Sort by issue number, then project path for consistency
6. Write merged list to file

### Task 2: Remove SaveTestResultsAsync and PromoteResultsAsync

**File**: `Tools/IssueRunner.Core/Commands/RunTestsCommand.cs`

**Current Implementation**:
- `SaveTestResultsAsync` writes to `test-passes.json` and `test-fails.json`
- `PromoteResultsAsync` moves results between pass/fail files

**Changes Required**:
1. Remove or deprecate `SaveTestResultsAsync` - no longer needed since `results.json` contains all information
2. Remove or deprecate `PromoteResultsAsync` - no longer needed
3. Remove calls to these methods from `ExecuteAsync`

**Implementation Steps**:
1. Comment out or remove `SaveTestResultsAsync` method
2. Comment out or remove `PromoteResultsAsync` method
3. Remove `await SaveTestResultsAsync(...)` call from `ExecuteAsync`
4. Remove `await PromoteResultsAsync(...)` call from `ExecuteAsync`
5. Update any console output that references these operations

**Note**: Keep the methods commented out initially for reference, can be fully removed later.

### Task 3: Update --rerun-failed to Use results.json

**File**: `Tools/IssueRunner.Core/Commands/RunTestsCommand.cs`

**Current Implementation**:
The `--rerun-failed` option reads from `test-fails.json`:

```csharp
var failedTests = await LoadTestResultsAsync(repositoryRoot, "test-fails.json", cancellationToken);
```

**Changes Required**:
1. Load from `results.json` instead
2. Filter `IssueResult` objects where `TestResult != "success"`
3. Extract issue numbers from filtered results

**Implementation Steps**:
1. Load `results.json` using `LoadPreviousResultsAsync`
2. Filter results where `TestResult` is not "success" (includes "fail", "not run", etc.)
3. Extract unique issue numbers from filtered results
4. Use these issue numbers to filter `issuesToRun` dictionary

### Task 4: Update TestResultDiffService to Use results.json

**File**: `Tools/IssueRunner.Core/Services/TestResultDiffService.cs`

**Current Implementation**:
Loads from `test-passes.json`, `test-fails.json`, `test-passes.baseline.json`, and `test-fails.baseline.json`.

**Changes Required**:
1. Load from `results.json` and `results-baseline.json` instead
2. Determine pass/fail status from `IssueResult.TestResult` field
3. Create dictionaries keyed by `Number|ProjectPath` for comparison

**Implementation Steps**:
1. Load `results.json` and `results-baseline.json` (create helper method similar to `LoadTestResultsAsync`)
2. For each file, create dictionary keyed by `$"{result.Number}|{result.ProjectPath}"`
3. Determine status: "success" if `TestResult == "success"`, otherwise use `TestResult` value (e.g., "fail", "not run")
4. Compare dictionaries to find differences
5. Classify changes using existing `DetermineChangeType` logic

**Key Changes**:
- Replace `LoadTestResultsAsync` calls with loading `results.json`/`results-baseline.json`
- Replace `TestResultEntry` with `IssueResult`
- Extract status from `IssueResult.TestResult` field

### Task 5: Update MainViewModel to Load from results.json

**File**: `Tools/IssueRunner.Gui/ViewModels/MainViewModel.cs`

**Current Implementation**:
The `LoadIssuesIntoViewAsync` method loads test results from `test-passes.json` and `test-fails.json`. The `LoadRepository` method also loads from these files for summary counts.

**Changes Required**:
1. Load from `results.json` instead of `test-passes.json`/`test-fails.json`
2. Extract test result from `IssueResult.TestResult` field
3. Handle multiple projects per issue (see Task 6)
4. Use `IssueResult.LastRun` for timestamp (after Task 7 adds this field)
5. Update summary counts to calculate from `results.json`

**Implementation Steps**:
1. Create helper method `LoadResultsAsync` to load `results.json` (similar to `LoadPreviousResultsAsync` in RunTestsCommand)
2. In `LoadIssuesIntoViewAsync`, load from `results.json` instead of pass/fail files
3. Group results by issue number
4. For each issue, determine which result to display (see Task 6)
5. Extract `TestResult` and `LastRun` fields
6. Build `resultsByIssue` dictionary with issue number as key
7. In `LoadRepository`, calculate summary counts from `results.json`:
   - Count results where `TestResult == "success"` for passed count
   - Count results where `TestResult != "success"` for failed count
   - Use domain model filtering instead of separate files

### Task 6: Handle Multiple Projects Per Issue

**Problem**: An issue can have multiple projects, each with its own `IssueResult` entry. The Issue List View shows one row per issue, so we need to decide which result to display.

**Options**:
1. **Show worst result**: If any project failed, show "fail". If all passed, show "success". If any not run, show "not run".
2. **Show most recent result**: Show the result from the most recently run project.
3. **Show primary project**: Show result from the first/main project (requires identifying primary project).

**Recommended Approach**: Show worst result (Option 1) as it gives the most useful information at a glance.

**Implementation Steps**:
1. After loading all results from `results.json`, group by issue number
2. For each issue group, determine the "worst" result using priority:
   - "fail" > "not run" > "success"
   - If multiple projects have same worst status, use the most recent one
3. Store the worst result in `resultsByIssue` dictionary

**Alternative**: If we want to show more detail, we could show "fail (2/3)" indicating 2 out of 3 projects failed, but this requires UI changes.

### Task 7: Add Timestamp to IssueResult (Required)

**File**: `Tools/IssueRunner.Core/Models/IssueResult.cs`

**Current State**: `IssueResult` doesn't have a timestamp field for when the test was run.

**Changes Required**:
1. Add `LastRun` property of type `string` (ISO 8601 format) to `IssueResult`
2. Set this property when creating `IssueResult` in `ProcessIssueAsync`
3. Use this timestamp when displaying "Last Run" in the Issue List View

**Implementation Steps**:
1. Add `[JsonPropertyName("last_run")] public string? LastRun { get; init; }` to `IssueResult`
2. Set `LastRun = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")` when creating results in `ProcessIssueAsync`
3. Update `MainViewModel` to use `result.LastRun` instead of deriving from file modification time

**Note**: This is now required since we're using `results.json` as the single source of truth and need accurate timestamps for each result.

### Task 8: Update SetBaselineAsync to Use results-baseline.json

**File**: `Tools/IssueRunner.Gui/ViewModels/MainViewModel.cs`

**Current Implementation**:
Copies `test-passes.json` to `test-passes.baseline.json` and `test-fails.json` to `test-fails.baseline.json`.

**Changes Required**:
1. Copy `results.json` to `results-baseline.json` instead
2. Remove copying of pass/fail baseline files

**Implementation Steps**:
1. Update `SetBaselineAsync` to copy `results.json` to `results-baseline.json`
2. Remove code that copies `test-passes.json` and `test-fails.json` to baseline files
3. Update log message to reflect new baseline file

### Task 9: Update TestStatusViewModel to Use results.json

**File**: `Tools/IssueRunner.Gui/ViewModels/TestStatusViewModel.cs`

**Current Implementation**:
Likely loads from `test-passes.json`/`test-fails.json` for baseline comparison.

**Changes Required**:
1. Load from `results.json` and `results-baseline.json` instead
2. Calculate pass/fail counts from `IssueResult` objects
3. Use domain model filtering

**Implementation Steps**:
1. Load `results.json` and `results-baseline.json`
2. Filter results where `TestResult == "success"` for pass counts
3. Filter results where `TestResult != "success"` for fail counts
4. Compare current vs baseline to find differences

### Task 10: Update Documentation

**File**: `README.md`

**File**: `README.md`

**Changes Required**:
1. Update "results.json" section to reflect incremental update behavior and that it's now the single source of truth
2. Add "results-baseline.json" section
3. Mark "test-passes.json", "test-fails.json", "test-passes.baseline.json", and "test-fails.baseline.json" as **DEPRECATED** or remove them entirely
4. Update all references to these deprecated files
5. Update "Test Result Files" section to reflect new approach
6. Update "Baseline Management" section to reference `results-baseline.json`

**Specific Updates**:
- Change "Updated: Every time `RunTestsCommand` completes a test run... The file is completely rewritten" to "Updated: Every time `RunTestsCommand` completes a test run... The file is incrementally updated - new results overwrite existing entries for the same issue+project combination, while preserving results for issues not in the current run"
- Add: "**Note**: `results.json` is now the single source of truth for all test results. The Issue List View, Test Status View, and all other components load from this file. Use domain model filtering (e.g., `TestResult == "success"`) to determine pass/fail status."
- Add new section for `results-baseline.json` explaining it replaces the old baseline files
- Mark old files as deprecated with migration note

### Task 11: Testing

**Test Scenarios**:
1. **First run**: Run tests for issues 1, 2, 3. Verify `results.json` contains only these 3 issues.
2. **Incremental update**: Run tests for issues 4, 5. Verify `results.json` now contains issues 1, 2, 3, 4, 5.
3. **Update existing**: Run tests for issue 2 again with different result. Verify issue 2's result is updated, others unchanged.
4. **Multiple projects**: Run test for issue with multiple projects. Verify Issue List View shows worst result.
5. **Issue List View**: Verify Issue List View displays test results from `results.json` correctly, including issues not in most recent run.
6. **--rerun-failed**: Verify `--rerun-failed` option reads from `results.json` and filters correctly.
7. **Baseline**: Set baseline, verify `results-baseline.json` is created. Run new tests, verify diff comparison works.
8. **TestStatus View**: Verify TestStatus View calculates pass/fail counts from `results.json` correctly.
9. **Summary counts**: Verify repository summary counts are calculated from `results.json` correctly.
10. **No old files**: Verify that `test-passes.json` and `test-fails.json` are no longer created/updated.

**Test Files to Create/Update**:
- Unit test for `SaveResultsAsync` merge logic
- Unit test for `--rerun-failed` reading from `results.json`
- Unit test for `TestResultDiffService` using `results.json`/`results-baseline.json`
- Integration test for incremental updates
- Integration test for baseline comparison
- GUI test for Issue List View displaying results from `results.json`
- GUI test for TestStatus View using new baseline files

## Implementation Order

1. **Task 7** (Add Timestamp): Do this first as it's required for all other tasks
2. **Task 1** (Update SaveResultsAsync): Core functionality change - make results.json incremental
3. **Task 2** (Remove SaveTestResultsAsync): Stop writing to old files
4. **Task 6** (Handle Multiple Projects): Logic needed for display
5. **Task 3** (Update --rerun-failed): Update command to use results.json
6. **Task 4** (Update TestResultDiffService): Update diff service for baseline comparison
7. **Task 5** (Update MainViewModel): Update GUI to load from results.json
8. **Task 8** (Update SetBaselineAsync): Update baseline creation
9. **Task 9** (Update TestStatusViewModel): Update status view
10. **Task 10** (Update Documentation): After implementation is complete
11. **Task 11** (Testing): Throughout implementation, but comprehensive testing at the end

## Edge Cases and Considerations

1. **Backward Compatibility**: 
   - Existing `results.json` files will work fine - they'll just be loaded and merged as-is
   - Existing `test-passes.json`/`test-fails.json` files will be ignored (can add migration logic if needed)
   - Existing baseline files can be migrated manually or left as-is (they won't be used)

2. **Empty results.json**: If file doesn't exist, treat as empty list (already handled in `LoadPreviousResultsAsync`)

3. **Corrupted results.json**: Handle JSON deserialization errors gracefully (already handled in `LoadPreviousResultsAsync`)

4. **Concurrent Updates**: If multiple test runs happen simultaneously, last write wins (acceptable for this use case)

5. **File Size**: Over time, `results.json` will grow. Consider cleanup/archival strategy if it becomes too large (future enhancement)

6. **Project Path Changes**: If a project path changes (e.g., folder renamed), it will be treated as a new entry. Old entry will remain (acceptable - can be cleaned up manually if needed)

7. **Migration from Old Files**: 
   - Option A: Add one-time migration logic to convert `test-passes.json`/`test-fails.json` to `results.json` format
   - Option B: Just stop using old files and let users regenerate results.json by running tests
   - Recommendation: Option B (simpler, less error-prone)

8. **Domain Model Filtering**: All pass/fail queries now use LINQ on `IssueResult` objects:
   - Pass: `results.Where(r => r.TestResult == "success")`
   - Fail: `results.Where(r => r.TestResult != "success")`
   - This is more flexible and maintainable than separate files

## Files to Modify

1. `Tools/IssueRunner.Core/Models/IssueResult.cs` - Add `LastRun` property (required)
2. `Tools/IssueRunner.Core/Commands/RunTestsCommand.cs` - Update `SaveResultsAsync`, remove `SaveTestResultsAsync`/`PromoteResultsAsync`, update `--rerun-failed`
3. `Tools/IssueRunner.Core/Services/TestResultDiffService.cs` - Update to use `results.json`/`results-baseline.json`
4. `Tools/IssueRunner.Gui/ViewModels/MainViewModel.cs` - Update `LoadIssuesIntoViewAsync`, `LoadRepository`, `SetBaselineAsync`
5. `Tools/IssueRunner.Gui/ViewModels/TestStatusViewModel.cs` - Update to use `results.json`/`results-baseline.json`
6. `README.md` - Update documentation, mark old files as deprecated
7. Test files (to be created/updated)

## Success Criteria

1. Running a subset of issues preserves results for other issues in `results.json`
2. Issue List View displays test results for all issues that have ever been tested, not just the most recent run
3. Multiple projects per issue are handled correctly (worst result shown)
4. `results.json` is the single source of truth - all components read from it
5. `results-baseline.json` is used for baseline comparisons
6. `test-passes.json` and `test-fails.json` are no longer created or updated
7. `--rerun-failed` option works correctly using `results.json`
8. Baseline comparison works correctly using `results.json`/`results-baseline.json`
9. Summary counts in repository status are calculated from `results.json`
10. Documentation accurately reflects the new behavior
11. All existing functionality continues to work (reports still generated, etc.)

