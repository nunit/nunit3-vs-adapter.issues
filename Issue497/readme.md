#  dotnet test with category filter is slow with a lot of tests

[Link to issue](https://github.com/nunit/nunit3-vs-adapter/issues/497)


Test using :
```cmd
dotnet test Issue497_Filtering.csproj --filter=TestCategory!=Slow -v:n -s .runsettings
```