using Antlr4.Runtime;
using Yail;
using Yail.Common;
using Yail.Grammar;

#if DEBUG
var input = File.ReadAllText("""C:\Users\Luuqe\RiderProjects\yail\Yail\Samples\test.yail""");
Console.WriteLine("[Debug mode]");
#else
var input = string.Empty;
if (args.Length < 1)
{
    ExceptionHelper.PrintError("Provide source file path.");
}
#endif

if (args.Length > 0)
{
    var filePath = args[0];
    if (!filePath.CheckExt())
    {
        ExceptionHelper.PrintError("Yail source files must end with '.y' or '.yail'.");
    }
    
    input = File.ReadAllText(filePath);
}

var packages = input.ExtractUsings();

var folderPath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE") ?? string.Empty, ".yail");
var files = FileExtensionHelper.GetFilesWithExtensions(folderPath, new[] { ".y", ".yail" });

foreach (var file in files)
{
    var fileName = Path.GetFileName(file).Split(".").First();
    if (packages.Contains(fileName))
    {
        var source = File.ReadAllText(file);
        input = string.Join("\n", source, input);
    }
}

input = input.RemovePackageAndUsingStatements();

var inputStream = new AntlrInputStream(input);

var lexer = new ExpressionsLexer(inputStream);
var tokenStream = new CommonTokenStream(lexer);
var parser = new ExpressionsParser(tokenStream);
//parser.AddErrorListener(); // TODO: future

var tree = parser.program();

#if DEBUG
//Console.WriteLine(tree.ToStringTree(parser)); 
#endif

var visitor = new ExpressionsVisitor();
visitor.Visit(tree);