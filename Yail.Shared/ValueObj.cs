namespace Yail.Shared;

public struct ValueObj
{
    public bool IsConst;
    public object? Value;
    public EDataType DataType;

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
}