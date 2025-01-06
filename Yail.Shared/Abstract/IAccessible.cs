namespace Yail.Shared.Abstract;

public interface IAccessible
{
    void Set(object index, ValueObj value);
    ValueObj Get(object index);
}