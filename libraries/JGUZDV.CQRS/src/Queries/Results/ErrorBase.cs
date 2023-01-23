using JGUZDV.CQRS.Queries.Results;

namespace JGUZDV.CQRS.Queries
{
    public class ErrorBase<T> : QueryResult<T>
    {
        internal ErrorBase(string failureCodes) : base()
        {
            FailureCode = failureCodes;
        }

        public string FailureCode { get; }
    }
}
