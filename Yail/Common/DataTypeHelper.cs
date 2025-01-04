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

    public static ValueObj? CastTo(this ValueObj value, string targetType)
    {
        var val = value.Value;
        
        switch (targetType)
        {
            case "i16":
                return null;
            
            case "i32":
                if (val is int intValue32)
                    return new ValueObj { IsConst = true, Value = intValue32, DataType = EDataType.Int32 };
                if (val is double doubleValue32)
                    return new ValueObj { IsConst = true, Value = (int)doubleValue32, DataType = EDataType.Int32 };
                if (val is char character)
                    return new ValueObj { IsConst = true, Value = (int)character, DataType = EDataType.Int32 };
                return null;

            case "i64":
                return null;

            case "double":
                if (val is int intValDouble)
                    return new ValueObj { IsConst = true, Value = (double)intValDouble, DataType = EDataType.Double };
                if (val is double doubleVal)
                    return new ValueObj { IsConst = true, Value = doubleVal, DataType = EDataType.Double };
                return null;

            case "string":
                if (val is string strVal)
                    return new ValueObj { IsConst = true, Value = strVal, DataType = EDataType.String };
                return new ValueObj { IsConst = true, Value = val!.ToString(), DataType = EDataType.String };

            case "bool":
                if (val is bool boolVal)
                    return new ValueObj { IsConst = true, Value = boolVal, DataType = EDataType.Boolean };
                return null;

            case "char":
                if (val is string strValChar && strValChar.Length == 1)
                    return new ValueObj { IsConst = true, Value = strValChar[0], DataType = EDataType.Char };
                return null;

            case "any":
                return value;

            case "void":
                return null;

            default:
                return null;
        }
    }
}