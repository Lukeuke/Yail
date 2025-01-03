using Yail.Shared;

namespace Yail.Common;

public static class DataTypeHelper
{
    public static EDataType ToDataType(this string val)
    {
        var dict = new Dictionary<string, EDataType>
        {
            { "i32", EDataType.Int32 },
            { "double", EDataType.Double },
            { "string", EDataType.String },
            { "bool", EDataType.Boolean },
            { "char", EDataType.Char },
            { "null", EDataType.Null },
            { "any", EDataType.Any },
            { "void", EDataType.Void },
        };

        if (!dict.TryGetValue(val, out var result))
        {
            throw new NotImplementedException("Data type not supported.");
        }

        return result;
    }
    
    public static EAccessModifier ToAccessLevel(this string val)
    {
        var dict = new Dictionary<string, EAccessModifier>
        {
            { string.Empty, EAccessModifier.Private },
            { "pv", EAccessModifier.Private },
            { "pub", EAccessModifier.Public }
        };

        if (!dict.TryGetValue(val, out var result))
        {
            throw new NotImplementedException("Access modifier not supported.");
        }

        return result;
    }
    
    public static EDataType DetermineResultType(ValueObj lhs, ValueObj rhs)
    {
        if (lhs.DataType == EDataType.Int32 || rhs.DataType == EDataType.Int32)
            return EDataType.Int32;
        if (lhs.DataType == EDataType.Boolean && rhs.DataType == EDataType.Boolean)
            return EDataType.Boolean;

        throw new InvalidOperationException("Unsupported types for operation.");
    }
}