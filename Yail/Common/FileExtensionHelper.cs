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
    
    public static string RemovePackageAndUsingStatements(this string text)
    {
        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        
        var output = lines.Where(line => 
            !(line.StartsWith("package ") || line.StartsWith("using "))
        ).ToList();
        
        return string.Join("\n", output);
    }
}