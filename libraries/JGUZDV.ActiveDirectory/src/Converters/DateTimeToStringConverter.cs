namespace JGUZDV.ActiveDirectory.Converters
{
    internal class DateTimeToStringConverter : IToStringConverter<DateTime>
    {
        public string Convert(DateTime value, string? outFormat)
            => value.ToString(outFormat ?? "O");
    }
    
    internal class DateTimeOffsetToStringConverter : IToStringConverter<DateTimeOffset>
    {
        public string Convert(DateTimeOffset value, string? outFormat)
            => value.ToString(outFormat ?? "O");
    }
}
