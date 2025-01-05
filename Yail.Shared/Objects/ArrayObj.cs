namespace Yail.Shared.Objects;

public class ArrayObj : ValueObj
{
    public ArrayObj(List<ValueObj> items)
    {
        Value = items;
        DataType = EDataType.Array;
    }

    public ValueObj Get(int index)
    {
        var items = Value as List<ValueObj>;
        if (index < 0)
        {
            index = items.Count + index;
        }
        return items[index];
    }

    public List<ValueObj> Get()
    {
        return Value as List<ValueObj>;
    }
    
    public void Set(int index, ValueObj value)
    {
        var items = Value as List<ValueObj>;

        if (index < 0)
        {
            index = items.Count + index;
        }
        
        if (index >= 0 && index < items.Count)
            items[index] = value;
    }

    public override void Print(bool newLine = false)
    {
        var x = Get().Select(x => x.Value.ToString()).ToArray();
        var output = $"[{string.Join(", ", x)}]";
        
        if (newLine)
            output += Environment.NewLine;
        
        Console.Write(output);
    }
}