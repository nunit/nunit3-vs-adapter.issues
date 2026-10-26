# nunit3-vs-adapter.issues

Repros of issues in the [NUnit3 VS Adapter](https://github.com/nunit/nunit3-vs-adapter): one folder for each issue
that has one. The docs page [How to Develop and Fix Bugs in NUnit](https://docs.nunit.org/articles/developer-info/Developing-NUnit.html)
describes the whole workflow; this page is a short summary.

## Using a repro

Each repro is a folder `Issue<number>`, for example `Issue1097` for
[issue 1097](https://github.com/nunit/nunit3-vs-adapter/issues/1097). It builds as it is. Use it on its own:

```cmd
cd Issue1097
dotnet test
```

- **Command line:** `dotnet test`, or the folder's `run_*.cmd` / `run_*.sh` scripts when it has them. To check a
  filter, use `dotnet vstest <dll> /ListFullyQualifiedTests` and `/TestCaseFilter:...`.
- **Visual Studio:** open the solution. A `.runsettings` in the folder is picked up automatically. For bugs in Test
  Explorer, use `te-dump` ([NUnit.Tools.TestExplorerDump](https://github.com/nunit/NUnit.Tools.TestExplorerDump)) to
  dump the tree before and after a run, also for single tests. Delete `.vs`, `bin` and `obj` first.
- **Adapter dumps:** set `DumpXmlTestDiscovery` and `DumpXmlTestResults` in the `.runsettings`; the dumps are written
  to `bin\<configuration>\<target framework>\Dump`.
- **An unreleased adapter:** copy the package to `C:\nuget`, add that feed to the folder's `NuGet.config`, and update
  with `dotnet outdated -u -pre Always -inc NUnit3TestAdapter`.

## Adding a repro

Make a folder `Issue<number>` with the smallest project that shows the bug, using the code from the issue where you
can. Use the current target framework and packages, unless the bug needs older ones, and always reference
`Microsoft.NET.Test.Sdk` and `coverlet.collector`. Add a comment that explains what the bug is, and link the folder
from the issue.

Optional files:

| File                        | Purpose                                                                       |
|-----------------------------|-------------------------------------------------------------------------------|
| `NuGet.config`              | Package sources, with `C:\nuget` when testing an unreleased adapter.          |
| `.runsettings`              | Settings the bug needs. Visual Studio only picks it up from the solution folder. |
| `run_<name>.cmd` and `.sh`  | Commands to run instead of plain `dotnet test`. Add both, the repros run on Windows and Linux. |
| Marker file                 | `ignore`, `explicit`, `wip`, `gui`, `closedasnotplanned` or `windows`, empty or `.md` with the reason. |

The `issue_metadata.json`, `issue_initialstate.json` and `issue_results.json` files are written by IssueRunner, see
below. They are information: the issue's state from GitHub, the packages the repro was made with, and the last
results.

## Running all repros with IssueRunner

[IssueRunner](https://github.com/nunit/nunit.tools.issuerunner) runs all or some repros and reports which pass. It is
useful to see if the repros still work, for example with a new adapter before a release. Clone it next to this
repository, build it, and run it from this repository's root:

```cmd
..\nunit.tools.issuerunner\build.cmd
..\nunit.tools.issuerunner\run-tests.cmd --issues 1097,782 --feed Local
..\nunit.tools.issuerunner\run-tests.cmd --scope Regression
```

- `--feed Local` adds `C:\nuget`, `--scope Regression` runs the closed issues, `--scope Open` the open ones.
- IssueRunner skips folders with an `ignore`, `explicit`, `wip`, `gui` or `closedasnotplanned` marker, also when you
  ask for them by number; `windows` folders are skipped with `--skip-netfx`.
- `EXPECT_TESTS=N` and similar comments in run scripts are not checked; IssueRunner only looks at the script's exit
  code.
- Its data, the central metadata and the results, is in `.nunit/IssueRunner`. `metadata sync-from-github` and
  `metadata sync-to-folders` update the metadata; `reset` sets the packages back to `issue_initialstate.json`.

The IssueRunner README has all commands and options.

## CI

The workflows in `.github/workflows` run IssueRunner on GitHub: the closed issues on each push (`regression-tests-*`),
and the open issues weekly (`open-issues-*`), on Linux for .NET and on Windows for .NET Framework. The report is in
each run's summary and artifacts.
