namespace Yail.Shared.Abstract;

public interface IInstantiable
{
    void Set(string variableName, ValueObj value);
    Dictionary<string, ValueObj> Get();
    ValueObj Get(string propName);
}