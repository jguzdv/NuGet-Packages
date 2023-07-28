namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class ByteBase64Converter : IPropertyConverter
{
    public IEnumerable<string> ConvertProperty(IEnumerable<object> values)
    {
        return values.OfType<byte[]>()
            .Select(Convert.ToBase64String);
    }
}
