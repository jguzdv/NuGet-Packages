namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

public class StringConverter : IPropertyConverter
{
    public Type PropertyType => typeof(string);

    public IEnumerable<string> OutputFormats => new[] { "lower", "upper" };

    public IEnumerable<string> ConvertProperty(IEnumerable<object> values, string? outFormat)
    {
        var result = values.OfType<string>();

        return outFormat switch
        {
            "lower" => result.Select(x => x.ToLowerInvariant()),
            "upper" => result.Select(x => x.ToUpperInvariant()),
            _ => result
        };
    }
}