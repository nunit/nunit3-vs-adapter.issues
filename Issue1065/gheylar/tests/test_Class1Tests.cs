using System;
using lib;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace tests;

[TestFixture]
public class Class1Tests
{
    [Test]
    public void Test()
    {
        // the exception just saves writing a lot of unrelated set up
        var exception = new Exception("expected exception");
        var builder = new Mock<IEndpointRouteBuilder>();
        builder.Setup(b => b.ServiceProvider).Throws(exception);
        Assert.That(() => Class1.MapHealthChecks(builder.Object), Throws.Exception.SameAs(exception));
    }
}