using Yail.Shared.Abstract;

namespace Yail.Shared.Objects;

public class ArrayObj : ValueObj, IAccessible
{
    public List<ValueObj> Items => Value as List<ValueObj>;
    
    public ArrayObj(List<ValueObj> items)
    {
        Value = items;
        DataType = EDataType.Array;
    }

    public ValueObj Get(object index)
    {
        var idx = (int)index;
        
        if (idx < 0)
        {
            idx = Items.Count + idx;
        }
        return Items[idx];
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

    public void Set(object index, ValueObj value)
    {
        var idx = (int)index;
        
        if (idx < 0)
        {
            idx = Items.Count + idx;
        }

        if (idx >= 0 && idx < Items.Count)
            Items[idx] = value;
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