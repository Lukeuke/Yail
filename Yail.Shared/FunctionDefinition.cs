namespace Yail.Shared;

public class FunctionDefinition
{
    public string Name { get; set; }
    public List<(string Type, string Name)> Parameters { get; set; }
    public EDataType ReturnType { get; set; }
    public object Body { get; set; }

    public FunctionDefinition(string name, EDataType returnType, List<(string, string)> parameters, object body)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
        Body = body;
    }
}