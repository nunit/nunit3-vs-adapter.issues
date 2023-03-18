# Issue [1078](https://github.com/nunit/nunit3-vs-adapter/issues/1078)


Run the repro with 

```cmd
dotnet test -s .runsettings  --logger "console;verbosity=normal"
```

alterntively run

```cmd
dotnet test --filter FullyQualifiedName!~Test1  --logger "console;verbosity=normal"
```