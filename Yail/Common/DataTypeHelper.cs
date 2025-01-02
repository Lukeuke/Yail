using Yail.Shared;

namespace Yail.Common;

public static class DataTypeHelper
{
    public static EDataType ToDataType(this string val)
    {
        var dict = new Dictionary<string, EDataType>
        {
            { "i32", EDataType.Integer },
            { "double", EDataType.Double },
            { "string", EDataType.String },
            { "bool", EDataType.Boolean },
            { "char", EDataType.Char },
            { "null", EDataType.Null },
            { "any", EDataType.Any }
        };

        if (!dict.TryGetValue(val, out var result))
        {
            throw new NotImplementedException("Data type not supported.");
        }

        return result;
    }
}