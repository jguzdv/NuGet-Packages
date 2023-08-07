namespace JGUZDV.ActiveDirectory.ClaimProvider.PropertyConverters;

internal class LongConverter : IPropertyConverter
{
    public Type PropertyType => typeof(long);

    public IEnumerable<string> OutputFormats => new[] { "FileTime", };

    public IEnumerable<string> ConvertProperty(IEnumerable<object> values, string? outFormat)
    {
        var result = values.Select(x =>
        {
            var t = x.GetType();
            var highPart = (int)t.InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, x, null);
            var lowPart = (int)t.InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, x, null);
            return highPart * ((long)uint.MaxValue + 1) + lowPart;
        });

        return outFormat switch
        {
            "FileTime" => result.Select(x => DateTimeOffset.FromFileTime(x).ToString("O")),
            _ => result.Select(x => x.ToString("0"))
        };
    }
}