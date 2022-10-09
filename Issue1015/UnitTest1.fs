module fsharp

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let Test1 () =
    Assert.Pass()

[<Test>]
let Test2 ([<Range(0, 8)>] r) =
    Assert.Pass()


