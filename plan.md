# Plan for IssueRunner

## Steps for each planning conversation

1. Read the plan below, feature by feature
2. For each feature - given the checkbox is not checked, figure out which tasks need to be done in order to implement the feature.  Add these tasks to the Tasklist section. The features are numbered, so add numbers as sub-numbers, like feature 1 would give tasks 1.1, 1.2, 1.3 and so on. The tasks should be simple work that can easily be done alone, and which does not require any of the other stuff to be done
3. While planning a feature, take into account how to test the feature and add that as tasks too.
4. Also figure out the sequence that tasks have to be implemented.
5. When the planning is done for a feature, check the checkbox in front of the feature.
6. If a feature is not understandable, stop processing and ask me to clarify the feature, and explain what is unclear.
7. Some features may be partially fixed. I will then have cleared the checkbox for the feature. That means there will be tasks that are done, and they don't need to be handled again. I will then change the feature plan, so just ensure that the tasks dont need to be undone, and add whatever nebuildw needs to be done.

Then Stop.

## Steps for each implementation task

1. Read the README.md
2. Pick the next unchecked task in the tasklist below
3. Implement the task
4. If the code can be unit tested, and there is no unit test for the change, implement a corresponding unit test
5. If a code change, then Compile the code, and redo until all errors and warnings are fixed
6. Implement all unit tests and headless tests if you change GUI
7. Run all unit tests, and ensure they pass.  Redo until they do.
8. Update the README.md with the user interaction information, or domain model changes for the implemented task
9. Run a markdown linter for the edited README.md file. If any warnings, fix them.
10. Mark the checkbox for the task as fixed.
11. Write to me that the task is fixed.
12. Proceed with the next task,

## Plan

[x] 1. There is no way to go back to the Run Tests list of issues after you have pressed the "Test Status". THis has to be either separate window, so that the upper right area can have multiple "windows", OR, it must be switchable using buttons.
[x] 2. Move the Run Tests button ANd the Options buttons to the Issue List window.Ensure that the options are stored in memory so that it remains the same for the next run.  Skip the selection of Scope there, as that can be set in the Issue list window filter section. Add a column and filter for Execution mode, Custom or Direct, which should be renamed to "Scripts only" and "dotnet test only" and "All". Execution mode should be renamed to TestTypes. It should be renamed both in the UI and in the CLI.
[x] 3. Now the "dotnet test" does both the restore, compile and test.  Split this into 3 steps, one for each, so that if one of these fails, that is status that should be reported as Test Result.  Document the different test result statuses.
[x] 4. The Issue list window should show a) the issue number should be a https link to the issue itself, b) Teh colours for the buttons should be more clear. Green and grey doesnt look like the right ones. Suggest some more standard GUI solution here for the colours.
[x] 5. Set up Avalonia headless testing framework for GUI testing using Avalonia.Headless.NUnit. This will enable automated testing of UI components, windows, buttons, and user interactions without requiring manual testing.
[x] 6. Repository status, upper left side.  It now shows Passed and Failed, sum = 99, whereas we have 130 in total.  Add lines for the rest, based on Skipped, or if there are other statuses, like COmpile failed, add that too.
[x] 7. The scopes and states and likewise in the IssueList and underlying code needs to be made more clear.  The Scope should be based on the underlying state of the issue, as we get them from github.  The Scope can then be Regression, meaning the issue is Closed. If the issue is not closed it is Open. USe these words (Regression, Open, and ofc All), dont append "Only".  The State should be based on what we see it has .  When it have just arrived from github, it will have metadata only.  If it doesnt have metadata it has been created before (which is the usual) we have synced with github and folders.  Anyway, the first state is then New. The next state is Synced. Then we see how it proceeds through the run.  Failed restore, Failed compile (aka build) and finnaly Runnable. Whether it fails or succeeds the test is shown in Test Result.  We have one more possible State, and that is Skipped - which is due to it having a marker file, which can be one of the marker signals we have introduced. In the reason text below, use this information (Ignored, Explicit, GUI and so on, see REadme.md for all of these).
[x] 8. Sync from Github.  Add a status dialog to start this,and which updates when running, which also allow for Cancel. Show the running number of issues synced, and whether they are found or not.  Right now, ALL issues fails, so the code is not working. Add an integration test for this too, to check that the code can sync a single issue.  Also allow for choosing to  syncing a subset of issues, like all those that don't have metadata but exist in the repo.  So All or Missing Metadata, and add Update existing, because some that where Open can have been Closed.  MOve the button to the IssueList, and remove from left side. Also, in the dialog add the button for Sync To FOlders, because it is only relevant after a Sync from Github.

## Task list

### Feature 1: Navigation between Issue List and Test Status views

- [x] 1.1 Add navigation buttons (e.g., "Issue List" and "Test Status") to switch between views in the main content area. Place buttons at the top of the main content area (Grid.Column="1", above the ContentControl), horizontally aligned. Buttons should be visible when either view is active to allow easy switching.
- [x] 1.2 Update MainViewModel to support view switching with a navigation state property
- [x] 1.3 Update MainWindow.axaml to display navigation buttons in the main content area header (above the ContentControl that displays CurrentView). Buttons should be styled consistently with the sidebar buttons and positioned in a horizontal StackPanel or Grid at the top of the main content Border (Grid.Row="0" of the main content Grid).
- [x] 1.4 Create unit test MainViewModelTests.cs: Test ShowTestStatusCommand sets CurrentView to TestStatusView
- [x] 1.5 Create unit test MainViewModelTests.cs: Test navigation command returns CurrentView to IssueListView
- [x] 1.6 Create unit test MainViewModelTests.cs: Test view switching state is maintained correctly
- [x] 1.7 Create headless test MainWindowHeadlessTests.cs: Test navigation buttons appear when Test Status view is active
- [x] 1.8 Create headless test MainWindowHeadlessTests.cs: Test clicking "Issue List" button switches view back to IssueListView
- [x] 1.9 Create headless test MainWindowHeadlessTests.cs: Test clicking "Test Status" button switches view to TestStatusView

### Feature 2: Move buttons and rename ExecutionMode to TestTypes

- [x] 2.1 Remove Run Tests and Options buttons from MainWindow sidebar (MainWindow.axaml)
- [x] 2.2 Use the Run Tests button existing on IssueListView.axaml, make necessary changes if needed.
- [x] 2.3 Add Options button to IssueListView.axaml
- [x] 2.4 Update IssueListViewModel to include  ShowOptionsCommand properties and ensure RunTestsCommand works on the exiting button.  Merge code for what is there already with the RunTestsCommand code.  We need only one.
- [x] 2.5 Update MainViewModel to pass RunTestsCommand and ShowOptionsCommand to IssueListViewModel
- [x] 2.6 Remove Scope selection from RunTestsOptionsDialog.axaml (since it's in the filter section)
- [x] 2.7 Update RunTestsOptionsViewModel to remove Scope property and related logic
- [x] 2.8 Add TestTypes column to IssueListView.axaml (display "Scripts" or  "DotNet test" ) depending on what is present in the test folder.
- [x] 2.9 Add TestTypes filter ComboBox to IssueListView.axaml filter section, with "All", "Scripts only" and "dotnet test only". Run Tests can be run with no filters (meaning All issues) or with any combination of filters.
- [x] 2.10 Update IssueListViewModel to include SelectedTestTypes property and filter logic
- [x] 2.11 Update IssueListItem model to include TestTypes property
- [x] 2.12 Update MainViewModel.LoadIssuesIntoViewAsync to populate TestTypes for each issue
- [x] 2.13 Remove ExecutionModein RunOptions.cs
- [x] 2.14 Update all references to ExecutionMode to use TestTypes (RunOptions, RunTestsCommand, etc.)
- [x] 2.15 Rename --execution-mode CLI option to --test-types in Program.cs
- [x] 2.16 Update CLI help text and README.md to reflect TestTypes instead of ExecutionMode
- [x] 2.17 Update wrapper scripts (run-tests.cmd/sh) to use --test-types instead of --execution-mode
- [x] 2.18 Create unit test IssueListViewModelTests.cs: Test ApplyFilters filters issues by SelectedTestTypes correctly
- [x] 2.19 Create unit test IssueListViewModelTests.cs: Test TestTypes property is populated correctly for issues with/without custom scripts
- [x] 2.20 Create unit test RunTestsOptionsViewModelTests.cs: Test Scope property is removed and not accessible
- [x] 2.21 Create unit test RunOptionsTests.cs: Test TestTypes enum values and property initialization
- [x] 2.22 Create unit test ProgramTests.cs: Test --test-types CLI option parsing and validation
- [x] 2.23 Create unit test RunTestsCommandTests.cs: Test FilterIssuesAsync filters by TestTypes correctly
- [x] 2.24 Create headless test IssueListViewHeadlessTests.cs: Test Run Tests button appears in IssueListView and is enabled
- [x] 2.25 Create headless test IssueListViewHeadlessTests.cs: Test Options button appears in IssueListView and opens RunTestsOptionsDialog
- [x] 2.26 Create headless test IssueListViewHeadlessTests.cs: Test TestTypes filter ComboBox filters issues correctly when selection changes
- [ ] 2.27 Create headless test IssueListViewHeadlessTests.cs: Test TestTypes column displays correct values ("Scripts", "DotNet test")

### Feature 3: Split dotnet test into restore, compile, and test steps

- [x] 3.1 Create new method ExecuteRestoreAsync in TestExecutionService to run `dotnet restore`
- [x] 3.2 Create new method ExecuteBuildAsync in TestExecutionService to run `dotnet build`
- [x] 3.3 Modify ExecuteDotnetTestAsync to run restore, build, then test as separate steps, but if restore fails, then the others should not run, and if build fails, then tests should not run.
- [x] 3.4 Update ITestExecutionService interface to return step-by-step results (restore success, build success, test success). Since these runs sequentially, the build and test should also have Not Run as a status.
- [x] 3.4.1  Also return the reason for the failure, so that we CAN display this if needed. The reason is the output from the relevant step.
- [x] 3.5 Update TestExecutionService return type to include restore and build status
- [x] 3.6 Update RunTestsCommand to handle and report restore/build/test failures separately
- [x] 3.7 Update IssueResult model to include RestoreResult, BuildResult, and TestResult separately
- [x] 3.8 Update BuildConclusion method to report which step failed (restore, build, or test)
- [x] 3.9 Update status dialog to show restore/build/test progress separately. Since they are a sequence of steps, report the latest that makes sense. restore fails, stop there, build fails stop there, and tests status is the last.
- [x] 3.10 Document test result statuses in README.md (restore failed, build failed, test failed, success)
- [x] 3.11 Create unit test TestExecutionServiceTests.cs: Test ExecuteRestoreAsync runs dotnet restore and returns correct exit code (tested via ExecuteTestsAsync)
- [x] 3.12 Create unit test TestExecutionServiceTests.cs: Test ExecuteBuildAsync runs dotnet build and returns correct exit code (tested via ExecuteTestsAsync)
- [x] 3.13 Create unit test TestExecutionServiceTests.cs: Test ExecuteDotnetTestAsync runs restore, build, then test in sequence
- [x] 3.14 Create unit test TestExecutionServiceTests.cs: Test ExecuteTestsAsync returns restore/build/test status separately when restore fails
- [x] 3.15 Create unit test TestExecutionServiceTests.cs: Test ExecuteTestsAsync returns restore/build/test status separately when build fails
- [x] 3.16 Create unit test TestExecutionServiceTests.cs: Test ExecuteTestsAsync returns restore/build/test status separately when test fails
- [x] 3.17 Create unit test RunTestsCommandTests.cs: Test BuildConclusion reports "restore failed" when restore step fails
- [x] 3.18 Create unit test RunTestsCommandTests.cs: Test BuildConclusion reports "build failed" when build step fails
- [x] 3.19 Create unit test RunTestsCommandTests.cs: Test BuildConclusion reports "test failed" when test step fails
- [x] 3.20 Create unit test RunTestsCommandTests.cs: Test BuildConclusion reports "success" when all steps pass
- [x] 3.21 Create headless test RunTestsStatusDialogHeadlessTests.cs: Test status dialog displays restore/build/test progress separately
- [x] 3.22 Create headless test RunTestsStatusDialogHeadlessTests.cs: Test status dialog shows "restore failed" when restore step fails
- [x] 3.23 Create headless test RunTestsStatusDialogHeadlessTests.cs: Test status dialog shows "build failed" when build step fails
- [x] 3.24 Create headless test RunTestsStatusDialogHeadlessTests.cs: Test status dialog shows "test failed" when test step fails

### Feature 4: Issue list improvements

- [x] 4.1 Update IssueListView.axaml to make issue number a clickable HyperlinkButton pointing to GitHub issue URL
- [x] 4.2 Update IssueListViewModel or MainViewModel to provide GitHub issue URL for each issue
- [x] 4.3 Research and suggest better button color scheme (replace green #107C10 and grey #666)
- [x] 4.4 Update button colors in MainWindow.axaml and IssueListView.axaml with new color scheme.  Ensure all buttons and filters a horisontally aligned
- [x] 4.5 Move Frameworks from Options to be a new Filter in the IssueListView. Values "All", ".Net", ".Net Framework".
- [x] 4.6 Move Issue List from option to the IssueList window window from Options,. Also make small button to clear the list.If there is anything in that window, use that, otherwise use the filters. Add the Issue list text box above the filters, and the clear button (e.g. an X), to the right of the list text box.
- [x] 4.7 Add number of Issues, which will vary with the filters of course, on top row, somewhere where it is suitable.
- [x] 4.8 Make application larger horizontally, so that there is room for all the filters and buttons.
- [ ] 4.20 Create unit test IssueListViewModelTests.cs: Test GitHub issue URL is generated correctly for each issue number
- [ ] 4.21 Create unit test MainViewModelTests.cs: Test GitHub URL generation uses correct repository URL from environment service
- [ ] 4.22 Create headless test IssueListViewHeadlessTests.cs: Test issue number is displayed as clickable HyperlinkButton
- [ ] 4.23 Create headless test IssueListViewHeadlessTests.cs: Test clicking issue number HyperlinkButton opens correct GitHub URL



### Feature 5: Set up Avalonia headless testing framework

- [x] 5.1 Create new test project IssueRunner.Gui.Tests.csproj (or add to existing IssueRunner.Tests project)
- [x] 5.2 Add Avalonia.Headless.NUnit NuGet package to test project (version matching Avalonia version 11.0.10)
- [x] 5.3 Add project reference to IssueRunner.Gui project in test project
- [x] 5.4 Create TestAppBuilder class with [assembly: AvaloniaTestApplication] attribute
- [x] 5.5 Configure TestAppBuilder.BuildAvaloniaApp() to use .UseHeadless() with appropriate options
- [x] 5.6 Create base test class or helper methods for common headless test setup (window creation, service mocking)
- [x] 5.7 Create InternalsVisibleTo attribute in IssueRunner.Gui to allow test project access to internal members
- [x] 5.8 Document headless testing setup in README.md or separate testing guide
- [x] 5.9 Test: Verify headless test project builds and runs a simple [AvaloniaTest] successfully

### Feature 6: Repository status - show all statuses

- [x] 6.1 Update MainViewModel.LoadRepository to calculate counts for all statuses: Passed, Failed, Skipped, Not Restored, Not Compiling, Not tested  (Not tested means there is no test results for the issue, but it compiles.)
- [x] 6.2 Add properties to MainViewModel for SkippedCount, NotCompilingCount, NotTestedCount, Not restored count.
- [x] 6.3 Update MainViewModel.SummaryText to include all status counts (Passed, Failed, Skipped, Not Compiling, Not Tested) and ensure total equals total issue count
- [x] 6.4 Update MainWindow.axaml Summary Section to display all status lines in a more structured format (consider using a Grid or StackPanel with multiple TextBlocks)
- [x] 6.5 Create unit test MainViewModelTests.cs: Test LoadRepository calculates all status counts correctly
- [x] 6.6 Create unit test MainViewModelTests.cs: Test SummaryText includes all status counts and totals correctly
- [x] 6.7 Create headless test MainWindowHeadlessTests.cs: Test repository status displays all status lines correctly

### Feature 7: Refactor Scope and State definitions

- [x] 7.1 Update TestScope enum in RunOptions.cs: Remove New, NewAndFailed, RegressionOnly, OpenOnly. Add Regression (for closed issues), Open (for open issues), keep All
- [x] 7.2 Update IssueListViewModel.AvailableScopes to return new scope values (All, Regression, Open)
- [x] 7.3 Update IssueListViewModel.ApplyFilters to filter by new scope values: Regression = State == "closed", Open = State == "open"
- [x] 7.4 Define new State enum/values: New (metadata only, just arrived from GitHub), Synced (has metadata and folder), Failed restore, Failed compile, Runnable, Skipped
- [x] 7.5 Update IssueListItem model to include new State property (separate from DetailedState which is for display)
- [x] 7.6 Update MainViewModel.LoadIssuesIntoViewAsync to determine State for each issue based on: metadata presence, build status, marker files - folder will always be present, since that is what we work out from. 
- [x] 7.7 Update IssueListViewModel.AvailableStates to return new state values (All, New, Synced, Failed restore, Failed compile, Runnable, Skipped)
- [x] 7.8 Update IssueListViewModel.ApplyFilters to filter by new state values
- [x] 7.9 Update IssueListItem.NotTestedReason to show marker file type (Ignored, Explicit, GUI, WIP) when Skipped state
- [x] 7.10 Update RunTestsCommand to use new scope values (Regression, Open, All) instead of old ones
- [x] 7.11 Update all references to old scope values throughout codebase (CLI, GUI, commands)
- [x] 7.12 Update README.md to document new Scope and State definitions
- [x] 7.13 Create unit test IssueListViewModelTests.cs: Test ApplyFilters filters by new Scope values (Regression, Open, All)
- [x] 7.14 Create unit test IssueListViewModelTests.cs: Test ApplyFilters filters by new State values
- [x] 7.15 Create unit test MainViewModelTests.cs: Test LoadIssuesIntoViewAsync determines State correctly for each issue
- [x] 7.16 Create unit test RunTestsCommandTests.cs: Test FilterIssuesAsync filters by new Scope values
- [x] 7.17 Create headless test IssueListViewHeadlessTests.cs: Test Scope filter ComboBox shows new values (All, Regression, Open)
- [x] 7.18 Create headless test IssueListViewHeadlessTests.cs: Test State filter ComboBox shows new values
- [x] 7.19 Create headless test IssueListViewHeadlessTests.cs: Test NotTestedReason displays marker file type when Skipped

### Feature 8: Sync from GitHub improvements

- [x] 8.1 Create SyncFromGitHubDialog.axaml with progress display, cancel button, and sync options
- [x] 8.2 Create SyncFromGitHubViewModel with properties: Progress, StatusText, IssuesSynced, IssuesFound, IssuesNotFound, IsRunning, CanCancel, SyncMode (All, MissingMetadata), UpdateExisting (bool)
- [x] 8.3 Add SyncToFolders button to SyncFromGitHubDialog (enabled only after successful sync)
- [x] 8.4 Update SyncFromGitHubCommand to accept CancellationToken and report progress via callback or events
- [x] 8.5 Update SyncFromGitHubCommand to support filtering: All issues or MissingMetadata only
- [x] 8.6 Update SyncFromGitHubCommand to support UpdateExisting option (update existing metadata vs skip)
- [x] 8.7 Update MainViewModel.SyncFromGitHubAsync to show SyncFromGitHubDialog and handle cancellation
- [x] 8.8 Update MainViewModel.SyncFromGitHubAsync to pass progress updates to dialog ViewModel
- [x] 8.9 Move Sync from GitHub button from MainWindow sidebar to IssueListView.axaml (add to filter section or as separate button)
- [x] 8.10 Remove Sync from GitHub button from MainWindow.axaml sidebar
- [x] 8.11 Update IssueListViewModel to include SyncFromGitHubCommand property
- [x] 8.12 Update MainViewModel to pass SyncFromGitHubCommand to IssueListViewModel
- [x] 8.13 Fix SyncFromGitHubCommand to properly handle GitHub API calls and error cases (currently all issues fail)
- [x] 8.14 Add integration test SyncFromGitHubCommandIntegrationTests.cs: Test sync can successfully sync a single issue
- [x] 8.15 Add integration test SyncFromGitHubCommandIntegrationTests.cs: Test sync with MissingMetadata filter only syncs issues without metadata
- [x] 8.16 Add integration test SyncFromGitHubCommandIntegrationTests.cs: Test sync with UpdateExisting updates existing metadata
- [x] 8.17 Create unit test SyncFromGitHubViewModelTests.cs: Test dialog properties update correctly during sync
- [x] 8.18 Create unit test SyncFromGitHubViewModelTests.cs: Test cancel button stops sync operation
- [x] 8.19 Create headless test IssueListViewHeadlessTests.cs: Test Sync from GitHub button appears in IssueListView
- [x] 8.20 Create headless test SyncFromGitHubDialogHeadlessTests.cs: Test dialog displays progress and status correctly
- [x] 8.21 Create headless test SyncFromGitHubDialogHeadlessTests.cs: Test cancel button stops sync and closes dialog
- [x] 8.22 Create headless test SyncFromGitHubDialogHeadlessTests.cs: Test SyncToFolders button is enabled after successful sync
- [x] 8.23 Add button for SyncToFolders in the Sync dialog box.  It should be enabled all time.  Remove same button from left side.

