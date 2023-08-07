using System.DirectoryServices;

namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public interface IPropertyConverter
{
    public Type PropertyType { get; }
    public IEnumerable<string> OutputFormats { get; }

    IEnumerable<string> ConvertProperty(IEnumerable<object> values, string outFormat);
}