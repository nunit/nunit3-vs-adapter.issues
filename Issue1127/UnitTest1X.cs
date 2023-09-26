namespace Issue1127.X;


using Xunit;
public class Tests
{
    [Theory]
    [InlineData(" ", false, false)]
    [InlineData(" ", true, true)]
    [InlineData(" ", false, true)]
    [InlineData(" ", true, false)]
    [InlineData("", false, false)]
    [InlineData("", true, true)]
    [InlineData("", false, true)]
    [InlineData("",true,false)]
    public void Test1(string a, bool b, bool c)
    {
        Assert.Equal(c,b);
    }

    [Fact]
    public void Test2()
    {
        Assert.Equal(1,1);
    }
}