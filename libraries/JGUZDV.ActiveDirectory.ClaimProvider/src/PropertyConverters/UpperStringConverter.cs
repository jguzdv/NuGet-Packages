namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public class UpperStringConverter : StringConverter, IPropertyConverter
{
    public override IEnumerable<string> ConvertProperty(IEnumerable<object> values)
    {
        return base.ConvertProperty(values)
            .Select(x => x.ToUpperInvariant());
    }
}
