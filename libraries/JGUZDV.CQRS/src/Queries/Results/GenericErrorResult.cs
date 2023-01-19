namespace JGUZDV.CQRS.Queries
{

    public class GenericErrorResult<T> : ErrorBase<T>
    {
        internal GenericErrorResult(string failureCode) : base(failureCode)
        { }
    }
}
