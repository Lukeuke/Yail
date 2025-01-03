namespace Yail.Shared;

public class FunctionDefinition
{
    public string Name { get; set; }
    public List<(string Type, string Name)> Parameters { get; set; }
    public EDataType ReturnType { get; set; }
    public EAccessModifier AccessModifier { get; set; }
    public object Body { get; set; }
    public string Package { get; }

    public FunctionDefinition(string name, EAccessModifier accessModifier, EDataType returnType, List<(string, string)> parameters, object body, string package)
    {
        Name = name;
        AccessModifier = accessModifier;
        ReturnType = returnType;
        Parameters = parameters;
        Body = body;
        Package = package;
    }
}