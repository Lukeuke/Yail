using System.Text;
using Newtonsoft.Json;
using Yail.Shared.Abstract;
using Yail.Shared.Helpers;

namespace Yail.Shared.Objects;

public class DictionaryObj : ValueObj, IAccessible
{
    private Dictionary<string, ValueObj> _dict => Value as Dictionary<string, ValueObj>;

    public DictionaryObj(Dictionary<string, ValueObj> dict)
    {
        DataType = EDataType.Dictionary;
        Value = dict;
    }

    public ValueObj Get(object key)
    {
        return _dict[(string)key];
    }

    public void Set(object index, ValueObj value)
    {
        var key = index as string;

        if (!_dict.TryGetValue(key, out _))
        {
            ExceptionHelper.PrintError($"Cannot find key '{key}'.");
        }

        _dict[key] = value;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(_dict);
    }

    public override void Print(bool newLine = false)
    {
        var sb = new List<string>();
        foreach (var (key, value) in _dict)
        {
            sb.Add($"\"{key}\" = {value.Value}");
        }

        var output = "{";
        output += string.Join(", ", sb);
        output += "}";
        
        if (newLine)
            output += Environment.NewLine;
        
        Console.Write(output);
    }
}