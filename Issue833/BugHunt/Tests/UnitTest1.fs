module Demos.Misc

(*

Bug: When this NuGet library
  <PackageReference Include="Shouldly" Version="4.0.3" />
is used in this project,
then you cannot double click on a test in the Text Explorer
in order to jump to the test.

For the failed test, you can still double click on the link
in the Stack Trace shown in the Test Detail Summary pane
in order to go to the line with the error.

*)

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let Test1 () =
    Assert.Pass()

[<Test>]
let Test2 () =
    () // dummy line here, to demonstrate that the error link goes to the next line
    Assert.Fail()

[<Test>]
let Test3 () =
    Assert.Inconclusive()
