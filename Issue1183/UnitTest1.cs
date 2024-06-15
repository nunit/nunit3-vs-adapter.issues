namespace Issue1183;

public class Tests
{
    [Test] public void Foo() { Assert.Pass(); }
    [Test] public void Bar() { Assert.Pass(); }
    
    [Category("FB")]
    [Test] public void FooBar() { Assert.Pass(); }
}