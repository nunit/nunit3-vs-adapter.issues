using System.Threading.Tasks;
using NUnit.Framework;
using WrapThat;

namespace Issue3694
{
    [Timeout(10000)]
    public class Tests
    {
        
        [Test]
        public async Task Test1()
        {
            Warn.If(YouKnow.TheAnswer, Is.Not.EqualTo(44));
            Assert.That(YouKnow.TheAnswer, Is.EqualTo(44));
        }
    }
}