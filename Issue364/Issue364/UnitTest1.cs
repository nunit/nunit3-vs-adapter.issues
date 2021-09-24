
using NUnit.Framework;
using SomeBaseNameSpace;

namespace SomeBaseNameSpace
{
    public abstract class EntityEditTests
    {
        [Test]
        public void DeleteEntity()
        {
            Assert.Pass();
        }
    }
}


namespace Issue364
{
    

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
