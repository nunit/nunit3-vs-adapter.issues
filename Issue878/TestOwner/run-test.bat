
vstest.console.exe "bin\x64\Debug\net48\TestOwner.dll" /logger:trx;LogFileName=without-owner.trx /TestCaseFilter:"TestCategory!=Foo"

vstest.console.exe "bin\x64\Debug\net48\TestOwner.dll" /logger:trx;LogFileName=with-owner.trx
