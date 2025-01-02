using Antlr4.Runtime;
using Yail;
using Yail.Grammar;

var input = File.ReadAllText("""C:\Users\Luuqe\RiderProjects\yail\Yail\Samples\working_on_variables.yail""");

if (args.Length > 0)
{
    var filePath = args[0];
    if (!filePath.EndsWith(".y") || !filePath.EndsWith(".yail"))
    {
        Console.Error.WriteLine("Yail source files must end with '.y' or '.yail'.");
        Environment.Exit(0);
    }
    
    input = File.ReadAllText(filePath);
}

var inputStream = new AntlrInputStream(input);

var lexer = new ExpressionsLexer(inputStream);
var tokenStream = new CommonTokenStream(lexer);
var parser = new ExpressionsParser(tokenStream);
//parser.AddErrorListener(); // TODO: future

var tree = parser.program();

var visitor = new ExpressionsVisitor();
visitor.Visit(tree);