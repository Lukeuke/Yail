using System.Text.RegularExpressions;
using Yail.Shared;
using Yail.Shared.Constants;

namespace Yail.Common;

public static class FileExtensionHelper
{
    public static bool CheckExt(this string filePath)
    {
        return filePath.EndsWith(".y") || filePath.EndsWith(".yail");
    }
    
    public static string[] GetFilesWithExtensions(string folderPath, string[] extensions)
    {
        var searchPatterns = new string[extensions.Length];
        for (var i = 0; i < extensions.Length; i++)
        {
            searchPatterns[i] = "*" + extensions[i];
        }

        var files = new List<string>();
        foreach (var pattern in searchPatterns)
        {
            try
            {
                files.AddRange(Directory.GetFiles(folderPath, pattern));
            }
            catch (DirectoryNotFoundException)
            {
                ExceptionHelper.PrintError($"Directory '{folderPath}' not found.");
            }
        }

        return files.ToArray();
    }
    
    public static string RemoveUsingStatements(this string text)
    {
        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        var output = lines.Where(line => 
            !line.StartsWith("using ")
        ).ToList();
        
        return string.Join(Environment.NewLine, output);
    }

    public static string RemoveCommentedContent(this string source)
    {
        var lines = source.Split(new[] { '\n', '\r' }, StringSplitOptions.None);
        var output = new List<string>();

        var inMultiLineComment = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.TrimEnd();

            if (inMultiLineComment)
            {
                if (trimmedLine.Contains("*/"))
                {
                    inMultiLineComment = false;
                }
            }
            else
            {
                var singleLineIdx = trimmedLine.IndexOf("//", StringComparison.Ordinal);

                var multiLineStartIdx = trimmedLine.IndexOf("/*", StringComparison.Ordinal);

                if (singleLineIdx != -1 && (multiLineStartIdx == -1 || singleLineIdx < multiLineStartIdx))
                {
                    output.Add(trimmedLine.Substring(0, singleLineIdx).TrimEnd());
                }
                else if (multiLineStartIdx != -1)
                {
                    output.Add(trimmedLine.Substring(0, multiLineStartIdx).TrimEnd());
                    inMultiLineComment = true;
                    
                    if (trimmedLine.IndexOf("*/", multiLineStartIdx, StringComparison.Ordinal) != -1)
                    {
                        inMultiLineComment = false;
                    }
                }
                else
                {
                    output.Add(trimmedLine);
                }
            }
        }

        return string.Join(Environment.NewLine, output);
    }

    public static string FillThePackageBeforeFunctionCall(this string text)
    {
        const string packagePattern = @"package\s+([a-zA-Z_][a-zA-Z0-9_]*)";
        var functionCallPattern = @"(?<!\w|\:\:|{0}\s)\b([a-zA-Z_][a-zA-Z0-9_]*)\s*\([^;]*\)\s*;".Replace("{0}", YailTokens.FunctionDeclaration);
        
        var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        string currentPackage = null;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            var packageMatch = Regex.Match(line, packagePattern);
            if (packageMatch.Success)
            {
                currentPackage = packageMatch.Groups[1].Value;
            }

            // Get built-in functions
            var yailFunctions = typeof(YailBuiltInFunctions);
            var fns = yailFunctions.GetProperties()
                .Select(propertyInfo => yailFunctions.GetProperty(propertyInfo.Name)
                    .GetValue(yailFunctions) as string)
                .Select(val => val + ')')
                .ToList();

            var functionMatch = Regex.Match(line, functionCallPattern);
            if (functionMatch.Success)
            {
                // if function == built-in -> continue
                var matchedFunction = functionMatch.Groups[1].Value + ")";
                
                if (fns.Contains(matchedFunction)) continue;
                
                var modifiedFunction = Regex.Replace(
                    line,
                    @"([a-zA-Z_][a-zA-Z0-9_]*)\s*\(",
                    $"{currentPackage}::$1("
                );
                lines[i] = modifiedFunction;
            }
        }

        return string.Join(Environment.NewLine, lines);
    }
}