using System.Runtime.Loader;

//All these work on V5.2
[TestFixture]
public class Tests
{
    string file = Path.GetFullPath("ClassLibrary.dll");

    //fails on 6.0
    [Test]
    public void WithLoadFrom()
    {
        var assembly = Assembly.LoadFrom(file);
        var type = assembly.GetType("Class1");
        ClassicAssert.IsTrue(typeof(Class1) == type);
    }

    [Test]
    public void WithGetAssemblyName()
    {
        var name = AssemblyName.GetAssemblyName(file);
        var assembly = Assembly.Load(name);
        var type = assembly.GetType("Class1");
        ClassicAssert.IsTrue(typeof(Class1) == type);
    }

    [Test]
    public void WithGetLoadContext()
    {
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly())!;
        var assembly = context.LoadFromAssemblyPath(file);
        var type = assembly.GetType("Class1");
        ClassicAssert.IsTrue(typeof(Class1) == type);
    }

    //fails on 6.0
    [Test]
    public void WithDefaultContext()
    {
        var context = AssemblyLoadContext.Default!;
        var assembly = context.LoadFromAssemblyPath(file);
        var type = assembly.GetType("Class1");
        ClassicAssert.IsTrue(typeof(Class1) == type);
    }
}