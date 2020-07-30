
`This has been resolved in version 3.17`

The following table shows whether XUnit/NUnit Console.WriteLine output shows when running in various scenarios.

|                                            | XUnit | NUnit |
|--------------------------------------------|-------|-------|
| dotnet test (SDK 3.1.101, Windows)         | No    | No    |
| dotnet test (SDK 3.1.101, Linux)           | Yes   | No    |
| JetBrains Rider (2019.3.3 Linux & Windows) | No    | Yes   |
| VS 16.4.5 + Resharper 2019.3.3             | No    | Yes   |
| VS 16.4.5 (native test runner)             | No    | Yes   |




