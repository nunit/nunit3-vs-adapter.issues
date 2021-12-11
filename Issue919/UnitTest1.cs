namespace Issue919;

//  public class Tests
//  {
    public class Foo
   {
       [TestCase(1)]
       public void Baz(int a)
       {
           Assert.Pass();
       }
   }
//}