using System.Runtime.Versioning;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class ByteGuidConverter : IPropertyConverter
{
    public IEnumerable<string> ConvertProperty(IEnumerable<object> values)
    {
        return values.OfType<byte[]>()
            .Select(x => new Guid(x).ToString());
    }
}
