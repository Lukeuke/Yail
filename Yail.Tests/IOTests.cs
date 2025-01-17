using Yail.Tests.Shared;

namespace Yail.Tests;

public class IOTests : BaseYailTest
{
    [Test]
    public void TestInputOutput()
    {
        var code = @"
            println(""Enter your name:"");
            var name = input();
            println(""Hello, "" + name + ""!"");
        ";

        var simulatedInput = "Bob";
        var output = RunCode(code, simulatedInput);

        Assert.That(output, Is.EqualTo("Enter your name:\nHello, Bob!\n"));
    }
}