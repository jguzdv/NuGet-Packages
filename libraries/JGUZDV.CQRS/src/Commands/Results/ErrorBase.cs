namespace JGUZDV.CQRS.Commands
{
    public class ErrorBase : CommandResult
    {
        internal ErrorBase(string failureCodes) : base()
        {
            FailureCode = failureCodes;
        }

        public string FailureCode { get; }
    }
}
