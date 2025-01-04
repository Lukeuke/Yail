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
        
        return string.Join("\n", output);
    }

    public static string RemoveComments(this string source)
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

        return string.Join("\n", output);
    }
}