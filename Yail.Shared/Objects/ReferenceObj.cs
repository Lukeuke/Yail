namespace Yail.Shared.Objects;

public class ReferenceObj : ValueObj
{
    public ValueObj Index { get; }
    
    public ReferenceObj(ValueObj accessible, ValueObj index)
    {
        Index = index;
        Value = accessible;
    }

    public override void Print(bool newLine = false)
    {
        var referenceTo = Value as ValueObj;
        var value = referenceTo!.Value!.ToString();
        
        if (newLine)
            value += Environment.NewLine;
        
        Console.Write(value);
    }
}