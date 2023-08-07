namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration
{
    public class ADPropertyInfo
    {
        public ADPropertyInfo(string propertyName, Type propertyType)
            : this(propertyName, propertyType, null) { }

        public ADPropertyInfo(string propertyName, Type propertyType, string? format)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            OutputFormat = format;
        }

        public string PropertyName { get; }
        public Type PropertyType { get; }
        public string? OutputFormat { get; }
    }
}
