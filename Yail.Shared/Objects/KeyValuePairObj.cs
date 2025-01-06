namespace Yail.Shared.Objects;

public class KeyValuePairObj : ValueObj
{
    public string Key { get; }
    
    public KeyValuePairObj(string key, ValueObj? value)
    {
        Key = key;
        Value = value;
    }
}