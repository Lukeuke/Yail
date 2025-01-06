using Yail.Shared;
using Yail.Shared.Abstract;
using Yail.Shared.Objects;

namespace Yail.Common.Extentions;

public static class ReferenceExtension
{
    public static void UpdateTheReferenceVariables(IAccessible accessible,Dictionary<string, ValueObj?> variables)
    {
        foreach (var (_, variable) in variables)
        {
            if (variable!.GetType() != typeof(ReferenceObj)) continue;
                    
            var reference = variable as ReferenceObj;
            var newVal = accessible.Get(reference!.Index.Value!);
            variable.Value = newVal;
        }
    }
}