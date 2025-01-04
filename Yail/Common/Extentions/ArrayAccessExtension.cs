using Yail.Shared;

namespace Yail.Common.Extentions;

public static class ArrayAccessExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="idx"></param>
    /// <returns>Char type</returns>
    public static ValueObj StringToChar(ValueObj value, int idx)
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
}