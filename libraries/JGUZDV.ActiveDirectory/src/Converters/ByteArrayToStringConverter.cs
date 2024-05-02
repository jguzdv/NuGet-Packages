using System.Security.Principal;

namespace JGUZDV.ActiveDirectory.Converters
{
    internal class ByteArrayToStringConverter : IToStringConverter<byte[]>
    {
        public string Convert(byte[] value, string? outFormat)
        {
            return outFormat switch
            {
                "Guid" => new Guid(value).ToString(),
                "SDDL" => new SecurityIdentifier(value, 0).ToString(),
                _ => System.Convert.ToBase64String(value)
            };
        }
    }
}
