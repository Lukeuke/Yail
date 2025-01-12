namespace Yail.Tests;

public static class TestExtension
{
    public static string StripWindows(this string value)
    {
        return value.Replace("\r", string.Empty);
    }
}