using Yail.Shared.Abstract;

namespace Yail.Shared.Objects;

public class ArrayObj : ValueObj, IAccessible
{
    public List<ValueObj> Items => Value as List<ValueObj>;
    
    public ArrayObj(IEnumerable<ValueObj> items)
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

    public EDataType GetItemsDataType()
    {
        return Get().Select(x => x.DataType).FirstOrDefault();
    }
    
    public override void Print(bool newLine = false)
    {
        var x = Get().Select(x => x.Value.ToString()).ToArray();

        if (GetItemsDataType() == EDataType.String)
        {
            x = x.Select(x => $@"""{x}""").ToArray();
        }
        if (GetItemsDataType() == EDataType.Char)
        {
            x = x.Select(x => $@"'{x}'").ToArray();
        }
        
        var output = $"[{string.Join(", ", x)}]";
        
        if (newLine)
            output += Environment.NewLine;
        
        Console.Write(output);
    }

    public void RemoveAt(int idxValue)
    {
        Items.RemoveAt(idxValue);
    }
    
    public static ArrayObj operator+ (ArrayObj obj1, ArrayObj obj2)
    {
        return new ArrayObj(obj1.Get().Concat(obj2.Get()));
    }
}