using Yail.Shared;
using Yail.Shared.Objects;

namespace Yail.Common.Extentions;

public static class SquareBracketAccessExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="idx"></param>
    /// <returns>Char type</returns>
    private static ValueObj StringToChar(ValueObj value, int idx)
    {
        var data = value.Value as string;

        if (idx < 0)
        {
            idx = data!.Length + idx;
        }
            
        return new ValueObj
        {
            DataType = EDataType.Char,
            IsConst = true, // maybe in future will be false
            Value = data![idx]
        };
    }
    
    public static ValueObj AccessString(ValueObj indexValue, ValueObj accessedValue, bool isReference)
    {
        if (indexValue.DataType != EDataType.Int32)
        {
            throw new Exception("Value must be i32");
        }

        var idx = indexValue.Value is int value ? value : 0;
        
        return StringToChar(accessedValue, idx);
    }
    
    public static ValueObj AccessArrayValue(ValueObj indexValue, ArrayObj accessedValue, bool isReference)
    {
        if (indexValue.DataType != EDataType.Int32)
        {
            throw new Exception("Value must be i32");
        }

        var idx = (int)indexValue.Value;
        
        var value = accessedValue.Get(idx);
        if (!isReference)
        {
            return value;
        }
        
        isReference = false;
        return new ReferenceObj(value, indexValue);
    }

    public static ValueObj AccessDictionaryValue(ValueObj indexValue, DictionaryObj dictionary, bool isReference)
    {
        if (indexValue.DataType != EDataType.String)
        {
            throw new Exception("Key value must be a string.");
        }

        var value = dictionary.Get((string)indexValue.Value!);
        
        if (!isReference) return value;
       
        isReference = false;
        return new ReferenceObj(value, indexValue);
    }
}