using Antlr4.Runtime;
using Yail.Grammar;

namespace Yail.Tests;

[TestFixture]
public class OperationsTest
{
    private string RunCode(string code)
    {
        using var output = new StringWriter();
        Console.SetOut(output);

        var inputStream = new AntlrInputStream(code);
        var lexer = new ExpressionsLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new ExpressionsParser(tokenStream);

        var tree = parser.program();

        var visitor = new ExpressionsVisitor();
        visitor.Visit(tree);
        
        return output.ToString();
    }
    
    [Test]
    public void TestAddition()
    {
        var code = @"
                var x = 3;
                var y = x + 3;
                println(y);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("6\r\n"));
    }
    
    [Test]
    public void TestSubtraction()
    {
        var code = @"
                var x = 5;
                var y = x - 3;
                println(y);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("2\r\n"));
    }
    
    [Test]
    public void TestMultiplication()
    {
        var code = @"
                var x = 5;
                var y = x * 3;
                println(y);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("15\r\n"));
    }
    
    [Test]
    public void TestDivision()
    {
        var code = @"
                var x = 20;
                var y = x / 4;
                println(y);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("5\r\n"));
    }
    
    [Test]
    public void TestModulo()
    {
        var code = @"
                var x = 20;
                var y = x % 2;
                var z = 15 % 2;
                println(y);
                println(z);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("0\r\n1\r\n"));
    }
}