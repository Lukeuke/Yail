using Antlr4.Runtime;
using Yail.Grammar;

namespace Yail.Tests.Shared;

public class BaseYailTest
{
    public string RunCode(string code, string input = "")
    {
        using var output = new StringWriter();
        Console.SetOut(output);

        using var inputReader = new StringReader(input);
        Console.SetIn(inputReader);

        var inputStream = new AntlrInputStream(code);
        var lexer = new ExpressionsLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new ExpressionsParser(tokenStream);

        var tree = parser.program();

        var visitor = new ExpressionsVisitor();
        visitor.Visit(tree);

        return output.ToString().StripWindows();
    }
}