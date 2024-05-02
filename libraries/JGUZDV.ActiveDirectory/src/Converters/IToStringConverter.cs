namespace JGUZDV.ActiveDirectory.Converters
{
    internal interface IToStringConverter<T>
    {
        public string Convert(T value, string? outFormat);

        public string Convert(object value, string? outFormat) 
            => Convert((T)value, outFormat);
    }
}