using NUnit.Framework;
using System;

namespace Issue516 {
    public class Tests {

        [TestCase( null )]
        [TestCase( "" )]
        [TestCase( " " )]
        [TestCase( "\t" )]
        [TestCase( "\r" )]
        [TestCase( "\n" )]
        [TestCase( "\r\n" )]
        public void Test1( string output ) {
            Console.Write( output );
            Assert.Pass();
        }

    }
}