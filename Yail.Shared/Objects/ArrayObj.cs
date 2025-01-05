namespace Yail.Shared.Objects;

public class ArrayObj : ValueObj
{
    public List<ValueObj> Items => Value as List<ValueObj>;
    
    public ArrayObj(List<ValueObj> items)
    {
        Value = items;
        DataType = EDataType.Array;
    }

    public ValueObj Get(int index)
    {
        if (index < 0)
        {
            index = Items.Count + index;
        }
        return Items[index];
    }

    public void Push(ValueObj value)
    {
        Items.Add(value);
    }

    public ValueObj Pop()
    {
        if (Items.Count == 0)
            throw new InvalidOperationException("Cannot pop from an empty array.");

        var lastItem = Items[^1];
        Items.RemoveAt(Items.Count - 1);
        return lastItem;
    }
    
    public List<ValueObj> Get()
    {
        return Items;
    }
    
    public void Set(int index, ValueObj value)
    {
        if (index < 0)
        {
            index = Items.Count + index;
        }
        
        if (index >= 0 && index < Items.Count)
            Items[index] = value;
    }

    public override void Print(bool newLine = false)
    {
        var x = Get().Select(x => x.Value.ToString()).ToArray();
        var output = $"[{string.Join(", ", x)}]";
        
        if (newLine)
            output += Environment.NewLine;
        
        Console.Write(output);
    }

    public void RemoveAt(int idxValue)
    {
        Items.RemoveAt(idxValue);
    }
}