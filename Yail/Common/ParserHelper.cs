using System.Text.RegularExpressions;

namespace Yail.Common;

public static class ParserHelper
{
    public static List<string> ExtractUsings(this string text)
    {
        var packageNames = new List<string>();
    
        const string pattern = @"using\s+([\""][^\""]+[\""]|\w+)";
    
        var matches = Regex.Matches(text, pattern);
    
        foreach (Match match in matches)
        {
            var value = match.Groups[1].Value;
        
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }
        
            packageNames.Add(value);
        }

        return packageNames;
    }
}