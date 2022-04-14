using NUnit.Framework;

namespace Issue919
{
    using NUnit.Framework;

namespace TestNUnit
{
   public class Foo
   {
       [TestCase(1)]
       public void Baz(int a)
       {
           Assert.Pass();
       }
   }
}
}