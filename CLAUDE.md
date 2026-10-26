# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

Repros of issues in the NUnit3 VS Adapter. Each `Issue<number>` folder is a small project that shows one issue and
builds as it is. See README.md, and the docs page
https://docs.nunit.org/articles/developer-info/Developing-NUnit.html for the whole workflow.

## Working with a repro

- Run a repro on its own: `dotnet test` in its folder, or its `run_*.cmd` / `run_*.sh` scripts.
- Test an unreleased adapter: copy the package to `C:\nuget`, add that feed to the folder's `NuGet.config`, update with
  `dotnet outdated -u -pre Always -inc NUnit3TestAdapter`. Never hand-edit package versions.
- Every test project references `Microsoft.NET.Test.Sdk` and `coverlet.collector`.
- Turn on `DumpXmlTestDiscovery` and `DumpXmlTestResults` in the folder's `.runsettings` to see what the adapter
  discovered, what Visual Studio sent to run (`VS_Input_TestCases` in the execution dump) and the filter it made.
- Check Test Explorer with `te-dump` (NUnit.Tools.TestExplorerDump): delete `.vs`, `bin` and `obj`, then
  `te-dump session`, or `te-dump start` / `run --pid <pid> --filter <text>` / `close --pid <pid>` for single tests.
  Its dumps go to a `te-dump` folder, which is in `.gitignore`.

## IssueRunner

IssueRunner lives in https://github.com/nunit/nunit.tools.issuerunner, not in this repository. It runs all or some
repros and reports which pass. Clone it next to this repository, build it with its `build.cmd`, and run
`..\nunit.tools.issuerunner\run-tests.cmd --issues <numbers>` from this repository's root. Its data is in
`.nunit/IssueRunner`. Marker files (`ignore`, `explicit`, `wip`, `gui`, `closedasnotplanned`) make it skip a folder;
`EXPECT_*` comments in run scripts are not checked. The CI workflows in `.github/workflows` check it out and run it.
