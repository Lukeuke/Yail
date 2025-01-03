using Antlr4.Runtime;

namespace Yail.Core;

public class YailErrorListener : IAntlrErrorListener<IToken>
{
    public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
        RecognitionException e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine($"Error at line {line}, position {charPositionInLine}: {msg}");
        Console.ResetColor();
    }
}