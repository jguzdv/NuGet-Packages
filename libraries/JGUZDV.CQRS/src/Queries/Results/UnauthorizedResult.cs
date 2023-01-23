namespace JGUZDV.CQRS.Queries
{
    public class UnauthorizedResult<T> : ErrorBase<T>
    {
        internal UnauthorizedResult() : base("AuthFailed") { }
    }
}
