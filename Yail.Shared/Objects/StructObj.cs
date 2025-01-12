using Yail.Shared.Abstract;

namespace Yail.Shared.Objects;

public class StructObj : ValueObj, IInstantiable
{
    public StructObj()
    {
        Value = new Dictionary<string, ValueObj>();
    }
    
    public required string Name { get; set; }
    public required bool IsPublic { get; set; }

    public void Set(string variableName, ValueObj value)
    {
        var list = Get();
        
        list.TryAdd(variableName, value);

        Value = list;
    }

    public Dictionary<string, ValueObj> Get()
    {
        return Value as Dictionary<string, ValueObj>;
    }

    public ValueObj Get(string propName)
    {
        return (Value as Dictionary<string, ValueObj>)[propName];
    }
}