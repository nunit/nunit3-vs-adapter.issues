using System;
using NUnit.Framework;

namespace Issue484
{
    [TestFixture]
    public class TestClass
    {
        [TestCase('\x01', ExpectedResult = "%01")]
        [TestCase('\x1F', ExpectedResult = "%1F")]
        [TestCase('\x00', ExpectedResult = "%00")]
        [TestCase('\x20', ExpectedResult = "%20")]
        [TestCase('\x80', ExpectedResult = "%80")]
        [TestCase('\x90', ExpectedResult = "%90")]
        public string Test1(char c)
        {
            return Uri.HexEscape(c);
        }

        [TestCase("ABC\x01", ExpectedResult = "ABC%01")]
        public string Test2(string s)
        {
            return Uri.EscapeDataString(s);
        }
    }
}