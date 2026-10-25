Manual check, not for IssueRunner: what does Visual Studio Test Explorer send to the adapter under MTP?

Background: PR 1502 / 1519. FullyQualifiedNameFilterParser.Unescape (the MTP fast path) runs
WebUtility.HtmlDecode and Regex.Unescape. If Test Explorer does not HTML-encode the names, a test
whose name contains `&amp;` cannot be selected.

Steps:

1. Open Issue1502.csproj in Visual Studio. dump.runsettings is set as the run settings and turns on DumpXmlTestResults.
2. In Test Explorer, run each StringArgument and CustomName case on its own (select one, Run).
3. Open bin\Debug\net10.0\Dump\E_Issue1502.dll.dump. Each run overwrites it, so read it after every run.
4. Compare <MsFilter> (the raw filter from Test Explorer) with <ParsedTestCases> (what the adapter made of it) and with the test name.
   <MsFilter> is only written on the Test Explorer path. If it is missing, Test Explorer did not use the MTP filter path.

Read the result:

- <MsFilter> contains `&amp;` or `&quot;`: Test Explorer HTML-encodes, HtmlDecode is needed.
- <MsFilter> has only VSTest escaping (`\(`, `\\`, `\|`): HtmlDecode and Regex.Unescape are wrong, use plain VSTest unescape.

Command line, for comparison. It goes through TestFilterParser, not the fast path, and writes <MicrosoftTestFilter> instead:

    dotnet run -- --settings dump.runsettings --filter 'FullyQualifiedName=Issue1502.FilterEscapingTests.StringArgument\("a &amp; b"\)'

Results 2026-10-25, adapter 6.3.1, VS 18 (the dump XML-escapes <MsFilter> once; values below are unescaped):

| Test                         | <MsFilter>                                     | <ParsedTestCases>                | Ran |
|------------------------------|------------------------------------------------|----------------------------------|-----|
| Name with & and &amp;        | `...Name with \& and \&amp;`                   | `...Name with & and &`           | no  |
| StringArgument("say \"hi\"") | `...StringArgument\("say \\"hi\\""\)`          | `...StringArgument("say \"hi\"")`| yes |
| StringArgument("a\nb")       | `...StringArgument\("a\\nb"\)`                | `...StringArgument("a\nb")`      | yes |

Test Explorer sends only VSTest escaping. HtmlDecode broke the first case. Fixed in nunit3-vs-adapter PR 1518 (main) and 1519 (tokenizer).
