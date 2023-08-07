using System.Runtime.Versioning;
using System.Security.Principal;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class ByteConverter : IPropertyConverter
{
    public Type PropertyType => typeof(byte[]);

    public IEnumerable<string> OutputFormats => new[] { "Base64", "Guid", "SDDL" };

    public IEnumerable<string> ConvertProperty(IEnumerable<object> values, string? outFormat)
    {
        var result = values.OfType<byte[]>();

        return outFormat switch
        {
            "Guid" => result.Select(x => new Guid(x).ToString()),
            "SDDL" => result.Select(x => new SecurityIdentifier(x,0).ToString()),
            _ => result.Select(Convert.ToBase64String)
        }; 
    }
}
