using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System.Runtime.CompilerServices;

namespace Issue4413And3936;

public class Tests
{
    [Test]
    public void SingleTest()
    {
        int nunitAge = 23;
        Assert.That(nunitAge, Is.GreaterThan(42).And.LessThan(85));
    }

    [Test]
    public void SingleTest1()
    {
        int nunitAge = 23;
        Verify.That(nunitAge, Is.GreaterThan(42).And.LessThan(85));
    }

    [Test]
    public void SingleTest2()
    {
        double nunitAge = 23.4;
        Verify.That(nunitAge, Is.GreaterThan(42.0).Within(0.2).And.LessThan(85.4), null);
    }

    [Test]
    public void SingleTest3()
    {
        int nunitAge = 23;
        Verify.That(nunitAge, Is.GreaterThan(42).And.LessThan(85), $"The age {nunitAge} fails to meet the objective");
    }


    [Test]
    public void Test1()
    {
        int age = 42;
        int employment = 5;
        Assert.That(age, Is.GreaterThan(2));
        Assert.Multiple(() =>
        {
            Verify.That(age, Is.EqualTo(2));
            Verify.That(age, Is.EqualTo(3));
            Verify.That(age, Is.EqualTo(42));
            Verify.That(employment, Is.GreaterThan(55));
            Verify.That(age, Is.LessThan(42));
            Verify.That(age, Is.EqualTo(4));
            Verify.That(employment, Is.EqualTo(55));
        });
    }
}


public class Verify
{
    public static void That<TActual>(
            TActual actual,
            IResolveConstraint expression,
            string? message = null,
            [CallerArgumentExpression("actual")] string? actualExpression = null,
            [CallerArgumentExpression("expression")] string? constraintExpression = null,
            params object?[]? args)
    {
        var expressionMessage = "Assert.That(" + actualExpression + ", " + constraintExpression + ")";
        string msg = message != null ? $"{message}\n{expressionMessage} " : expressionMessage;
        var constraint = expression.Resolve();
        // Assert.IncrementAssertCount();
        var result = constraint.ApplyTo(actual);
        if (result.IsSuccess)
            return;
        MessageWriter writer = new TextMessageWriter(msg, args);
        result.WriteMessageTo(writer);
        Assert.Fail(writer.ToString(), args);
    }
}


