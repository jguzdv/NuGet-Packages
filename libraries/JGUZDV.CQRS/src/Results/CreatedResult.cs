namespace JGUZDV.CQRS
{
    public class CreatedResult : SuccessResult { }

    public class CreatedResult<T> : CreatedResult
    {
        internal CreatedResult(T value) : base()
        {
            Value = value;
        }

        public T Value { get; }
    }
}
