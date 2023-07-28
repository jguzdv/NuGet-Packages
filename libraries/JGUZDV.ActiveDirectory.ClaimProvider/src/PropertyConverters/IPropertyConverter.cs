using System.DirectoryServices;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public interface IPropertyConverter
{
    IEnumerable<string> ConvertProperty(IEnumerable<object> values);
}
