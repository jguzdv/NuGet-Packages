namespace JGUZDV.CQRS.Queries
{
    public class NotFoundResult<T> : ErrorBase<T>
    {
        internal NotFoundResult() : base("NotFound")
        { }
    }
}
