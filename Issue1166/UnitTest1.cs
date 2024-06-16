using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace DemoRSharperTestrunProblem
{
    [TestFixture]
    public class DemoProblemTestRunner
    {
        [Test]
        [TestCase(Language.DE, true)]
        [TestCase(Language.EN, false)]
        public void TestMethod(Language language, bool expected)
        {
            var logic = new BizLogic();

            var result = logic.Method(language);
            ClassicAssert.AreEqual(expected, result);
        }


        [Test]
        [TestCase(1, true)]
        [TestCase(2, false)]
        public void TestMethod(int languageCode, bool expected)
        {
            var logic = new BizLogic();

            var result = logic.Method((Language)languageCode);

            ClassicAssert.AreEqual(expected, result);
        }
    }

    public class BizLogic
    {
        public bool Method(Language input)
        {
            if ((int)input == 1)
            {
                return true;
            }

            return false;
        }
    }

    public enum Language
    {
        DE = 1,
        EN = 2
    }
}