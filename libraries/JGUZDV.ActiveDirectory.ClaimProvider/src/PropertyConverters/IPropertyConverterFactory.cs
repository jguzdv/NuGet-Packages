namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public interface IPropertyConverterFactory
{
    IPropertyConverter GetConverter(string propertyName, string? outputFormat);
}
