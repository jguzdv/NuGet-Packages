namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyReader
{
    public interface IPropertyConverterFactory
    {
        IPropertyConverter GetConverter(string propertyName);
    }
}
