using Yail.Shared;

namespace Yail.Common;

public static class ExceptionHelper
{
    public static void ThrowIfNull(this ValueObj? value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("Value was null.");
        }
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
        Environment.Exit(-1);
    }
}