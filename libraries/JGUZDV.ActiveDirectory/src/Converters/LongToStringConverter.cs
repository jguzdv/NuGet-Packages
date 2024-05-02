namespace JGUZDV.ActiveDirectory.Converters
{
    internal class LongToStringConverter : IToStringConverter<long>
    {
        public string Convert(long value, string? outFormat)
            => value.ToString(outFormat ?? "0");
    }
}
