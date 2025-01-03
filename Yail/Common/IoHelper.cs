using Yail.Shared;

namespace Yail.Common;

public class IoHelper
{
    public static ValueObj ParseInt(ValueObj value)
    {
        if (value.DataType != EDataType.String)
        {
            throw new InvalidOperationException("Expected a string for parsing.");
        }

        if (int.TryParse((string)value.Value, out var result))
        {
            return new ValueObj
            {
                DataType = EDataType.Int32,
                Value = result
            };
        }

        if (double.TryParse((string)value.Value, out var result2))
        {
            return new ValueObj
            {
                DataType = EDataType.Int32,
                Value = (int)result2
            };
        }
        
        throw new InvalidOperationException("Invalid i32/double string.");
    }

    public static ValueObj ParseDouble(ValueObj value)
    {
        if (value.DataType != EDataType.String)
        {
            throw new InvalidOperationException("Expected a string for parsing.");
        }

        if (double.TryParse((string)value.Value, out var result))
        {
            return new ValueObj
            {
                DataType = EDataType.Double,
                Value = result
            };
        }

        throw new InvalidOperationException("Invalid double string.");
    }
    
    public static ValueObj ParseBool(ValueObj value)
    {
        if (value.DataType != EDataType.Boolean)
        {
            throw new InvalidOperationException("Expected a string for parsing.");
        }

        if (bool.TryParse((string)value.Value, out var result))
        {
            return new ValueObj
            {
                DataType = EDataType.Boolean,
                Value = result
            };
        }

        throw new InvalidOperationException("Invalid bool string.");
    }

    public static ValueObj ToString(ValueObj value)
    {
        return new ValueObj
        {
            DataType = EDataType.String,
            Value = (string)value.Value!
        };
    }
}