namespace Yail.Shared;

public class ClassDefinition
{
    public string ClassName { get; set; }
    public string AccessLevel { get; set; }
    public List<string> Methods { get; set; } = new List<string>();
    public List<string> Fields { get; set; } = new List<string>();
}