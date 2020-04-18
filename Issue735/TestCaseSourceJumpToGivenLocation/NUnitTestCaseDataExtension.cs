using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Framework
{
    public static class NUnitTestCaseDataExtension
    {
        const string CodeFilePath = "_CodeFilePath";
        const string LineNumber = "_LineNumber";
        public static TestCaseData SetSourceLocationEx(this TestCaseData td,string path,int lineNumber=1)
        {
            td.Properties.Add(CodeFilePath, path);
            td.Properties.Add(LineNumber, lineNumber);
            return td;
        }
    }
}
