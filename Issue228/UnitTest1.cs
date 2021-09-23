using NUnit.Framework;

namespace Issue228
{
    abstract class EntityEditTests
    {
        [Test]
        public void DeleteEntity()
        {
            Assert.Pass();
        }
    }

    [TestFixture]
    class DocumentTests : EntityEditTests
    {
        [Test]
        public void SignDocument()
        {
            Assert.Pass();
        }
    }
}
