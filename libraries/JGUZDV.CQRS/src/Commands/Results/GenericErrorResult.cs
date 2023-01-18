namespace JGUZDV.CQRS.Commands
{

    public class GenericErrorResult : ErrorBase
    {
        internal GenericErrorResult(string failureCode) : base(failureCode)
        { }
    }
}
