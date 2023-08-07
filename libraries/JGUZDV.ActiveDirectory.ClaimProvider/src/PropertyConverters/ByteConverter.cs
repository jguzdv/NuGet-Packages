using System.Runtime.Versioning;
using System.Security.Principal;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

[SupportedOSPlatform("windows")]
internal class ByteConverter : IPropertyConverter
{
    public Type PropertyType => typeof(byte[]);

    public IEnumerable<string> OutputFormats => new[] { "Base64", "Guid", "SID" };

    public IEnumerable<string> ConvertProperty(IEnumerable<object> values, string outFormat)
    {
        var result = values.OfType<byte[]>();

        return outFormat switch
        {
            "Guid" => result.Select(x => new Guid(x).ToString()),
            "SID" => result.Select(x => new SecurityIdentifier(x,0).ToString()),
            _ => result.Select(Convert.ToBase64String)
        }; 
    }
}
