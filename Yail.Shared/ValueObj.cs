namespace Yail.Shared;

public class ValueObj
{
    public ValueObj()
    {
        
    }
    public ValueObj(object? value, EDataType dataType, bool isConst = false)
    {
        Value = value;
        DataType = dataType;
        IsConst = isConst;
    }

    public ValueObj(EDataType dataType, bool isConst = false)
    {
        DataType = dataType;
        IsConst = isConst;
        
        switch (dataType)
        {
            case EDataType.Int32:
                Value = 0;
                break;
            case EDataType.Boolean:
                Value = false;
                break;
            case EDataType.String:
                Value = string.Empty;
                break;
            case EDataType.Double:
                Value = 0.0D;
                break;
            default:
                Value = null;
                break;
        }
    }
    
    public bool IsConst { get; set; }
    public object? Value { get; set; }
    public EDataType DataType { get; set; }

    public TValue? GetValue<TValue>()
    {
        return Value is null ? default : (TValue)Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is ValueObj other)
        {
            return IsConst == other.IsConst &&
                   Equals(Value, other.Value) &&
                   DataType == other.DataType;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsConst, Value, DataType);
    }

    public override string ToString()
    {
        return $"ValueObj {{ IsConst = {IsConst}, Value = {Value}, DataType = {DataType} }}";
    }

    public virtual void Print(bool newLine = false)
    {
        var val = Value;

        if (newLine)
            val += Environment.NewLine;
        
        Console.Write(val);
    }
    
    public bool ValueEquals(object? obj)
    {
        if (obj is ValueObj other)
        {
            return Value.GetType() == other.Value.GetType() && DataType == other.DataType;
        }
        return false;
    }
}