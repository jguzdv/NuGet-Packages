using System.DirectoryServices;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public interface IPropertyConverter
{
    public string ConverterName { get; }

    IEnumerable<string> ConvertProperty(IEnumerable<object> values);
}
