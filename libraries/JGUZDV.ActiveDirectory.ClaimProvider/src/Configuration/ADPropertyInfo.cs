namespace JGUZDV.ActiveDirectory.ClaimProvider.Configuration
{
    public class ADPropertyInfo
    {
        public ADPropertyInfo(string propertyName, Type propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
        }

        public string PropertyName { get; }
        public Type PropertyType { get; }
    }
}
