namespace JGUZDV.CQRS.Commands
{
    public class ConflictResult : ErrorBase
    {
        internal ConflictResult(string failureCode)
            : base(failureCode) { }
    }
}
