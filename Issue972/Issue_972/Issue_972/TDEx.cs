using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using NUnit.Framework;

namespace Issue_972
{
    public class MyTests
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(12, 3).Returns(4);
                yield return new TestCaseData(12, 2).Returns(6);
                yield return new TestCaseData(12, 4).Returns(3);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public int DivideTest(int n, int d)
        {
            return n / d;
        }
    }

    public class MyTestsDT
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(1)).Returns(1);
                yield return new TestCaseData(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(2)).Returns(2);
                yield return new TestCaseData(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(3)).Returns(3);
            }
        }
        [Category("WhateverTime")]
        [TestCaseSource(nameof(TestCases))]
        public int DateTimeTEsts(DateOnly n1, DateOnly n2)
        {
            return n2.DayNumber-n1.DayNumber;
        }
    }

    public class MyTestsDTFails
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(DateTime.Now, DateTime.Now.AddDays(1)).Returns(1);
                yield return new TestCaseData(DateTime.Now, DateTime.Now.AddDays(2)).Returns(2);
                yield return new TestCaseData(DateTime.Now, DateTime.Now.AddDays(3)).Returns(3);
            }
        }
        [Category("WhateverTime")]
        [TestCaseSource(nameof(TestCases))]
        public int DateTimeTEsts(DateTime n1, DateTime n2)
        {
            return (n2 - n1).Days;
        }
    }


}
