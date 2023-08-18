namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class IntConverter : IPropertyConverter
{
    public Type PropertyType => typeof(int);

    public IEnumerable<string> OutputFormats => Array.Empty<string>();

    public IEnumerable<string> ConvertProperty(IEnumerable<object> values, string? outFormat)
    {
        return values.OfType<int>()
            .Select(x => x.ToString("0"));
    }
}