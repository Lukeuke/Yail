namespace Yail.Shared.Objects;

public class ArrayObj : ValueObj
{
    public List<ValueObj> Items { get; }

    public ArrayObj(List<ValueObj> items)
    {
        Items = items;
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

    public void Set(int index, ValueObj value)
    {
        if (index < 0)
        {
            index = Items.Count + index;
        }
        
        if (index >= 0 && index < Items.Count)
            Items[index] = value;
    }
}