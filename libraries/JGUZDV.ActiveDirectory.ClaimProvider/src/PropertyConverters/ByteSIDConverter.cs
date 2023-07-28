using System.Runtime.Versioning;
using System.Security.Principal;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

[SupportedOSPlatform("windows")]
internal class ByteSIDConverter : IPropertyConverter
{
    public virtual IEnumerable<string> ConvertProperty(IEnumerable<object> values)
    {
        return values.OfType<byte[]>()
            .Select(x => new SecurityIdentifier(x, 0).ToString());
    }
}
