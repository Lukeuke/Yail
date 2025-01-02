namespace Yail.Tests;

public class Tests
{
    private ExpressionsVisitor Sut { get; set; }
    
    [SetUp]
    public void Setup()
    {
        Sut = new ExpressionsVisitor();
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}