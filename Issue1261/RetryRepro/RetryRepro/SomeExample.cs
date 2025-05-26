using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetryRepro
{
    public class SomeExampleTests<T>
    {
        [TestCase(42)]
        [TestCase("Hi")]
        [TestCase(3.14)]
        public void TestMethod(T value)
        {
            // Arrange
            var sut = new SomeExample<T>();
            // Act
            var result = sut.DoSomething(value);
            // Assert
            Assert.That(result, Is.EqualTo(value));
        }
    }
}

public class SomeExample<T>
{
    public T DoSomething(T value)
    {
        // Simulate some processing
        return value;
    }
}
