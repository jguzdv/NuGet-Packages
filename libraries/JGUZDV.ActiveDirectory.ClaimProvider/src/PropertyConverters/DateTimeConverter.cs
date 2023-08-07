namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class DateTimeConverter : IPropertyConverter
{
    public Type PropertyType => typeof(DateTime);

    public IEnumerable<string> OutputFormats => Array.Empty<string>();

    public IEnumerable<string> ConvertProperty(IEnumerable<object> values, string? outFormat)
    {
        return values.OfType<DateTime>()
            .Select(x => x.ToString("O"));
    }
}