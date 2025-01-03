using System.Text.RegularExpressions;

namespace Yail.Common;

public static class ParserHelper
{
    public static List<string> ExtractUsings(this string text)
    {
        var packageNames = new List<string>();
        
        var pattern = @"using (\w+)";
        
        var matches = Regex.Matches(text, pattern);
        
        foreach (Match match in matches)
        {
            packageNames.Add(match.Groups[1].Value);
        }

        return packageNames;
    }
}