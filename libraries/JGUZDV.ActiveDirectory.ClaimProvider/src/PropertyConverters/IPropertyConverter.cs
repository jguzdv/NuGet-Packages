using System.DirectoryServices;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyReader
{
    public interface IPropertyConverter
    {
        IEnumerable<string> ConvertProperty(PropertyValueCollection propertyValues);
    }
}
