using Antlr4.Runtime;
using Yail;
using Yail.Common;
using Yail.Core;
using Yail.Grammar;

#if DEBUG
var filePath = """C:\Users\Luuqe\RiderProjects\yail\Yail\Samples\main.yail""";
var input = File.ReadAllText(filePath);
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("[Debug mode]");
Console.ResetColor();
#else
var input = string.Empty;
var filePath = string.Empty;
if (args.Length < 1)
{
    ExceptionHelper.PrintError("Provide source file path.");
}
#endif

if (args.Length > 0)
{
    filePath = args[0];
    if (!filePath.CheckExt())
    {
        ExceptionHelper.PrintError("Yail source files must end with '.y' or '.yail'.");
    }
    
    input = File.ReadAllText(filePath);
}

var packages = input.ExtractUsings();

var folderPath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE") ?? string.Empty, ".yail", "lib");
var files = FileExtensionHelper.GetFilesWithExtensions(folderPath, new[] { ".y", ".yail" });

FillBuiltInPackages();

FillExternalPackages();

input = input
    .RemoveCommentedContent()
    .RemoveUsingStatements();
    
if (args.Any(x => x == "disable-explicit-function-calls"))
{
    input = input.FillThePackageBeforeFunctionCall();
}

var inputStream = new AntlrInputStream(input);

var lexer = new ExpressionsLexer(inputStream);
var tokenStream = new CommonTokenStream(lexer);
var parser = new ExpressionsParser(tokenStream);

if (args.Any(x => x == "enable-errors"))
{
    parser.AddErrorListener(new YailErrorListener());
}

#if DEBUG
parser.AddErrorListener(new YailErrorListener());
#endif

var tree = parser.program();

#if DEBUG
Console.WriteLine(tree.ToStringTree(parser)); 
#endif

var visitor = new ExpressionsVisitor();
visitor.Visit(tree);

void FillBuiltInPackages()
{
    foreach (var file in files)
    {
        var fileName = Path.GetFileName(file).Split(".").First();
        if (packages.Contains(fileName))
        {
            var source = File.ReadAllText(file);
            input = string.Join("\n", source, input);
        }
    }
}

void FillExternalPackages()
{
    foreach(var extPkg in packages.Where(x => x.EndsWith(".yail") || x.EndsWith(".y"))) 
    {
        if (Environment.OSVersion.VersionString.Contains("Windows"))
        {
            var paths = filePath.Split("\\")[.. ^1];
            var projectDirectoryPath = string.Join( '\\', paths);

            var pkgSource = File.ReadAllText(Path.Combine(projectDirectoryPath, extPkg));
            input = string.Join(Environment.NewLine, pkgSource, input);
        }
    }
}