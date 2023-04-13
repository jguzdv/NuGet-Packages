using System.ComponentModel.DataAnnotations;

namespace JGUZDV.CQRS
{
    public class ErrorBase : HandlerResult
    {
        internal ErrorBase(string failureCodes) : base()
        {
            FailureCode = failureCodes;
        }

        public string FailureCode { get; }
    }


    public class GenericErrorResult : ErrorBase
    {
        internal GenericErrorResult(string failureCode) : base(failureCode)
        { }
    }


    public class CanceledResult : ErrorBase
    {
        public CancellationToken CancellationToken { get; }

        internal CanceledResult(CancellationToken cancellationToken) : base("Canceled")
        {
            CancellationToken = cancellationToken;
        }
    }


    public class ConflictResult : ErrorBase
    {
        internal ConflictResult(string failureCode)
            : base(failureCode) { }
    }


    public class NotFoundResult : ErrorBase
    {
        internal NotFoundResult() : base("NotFound")
        { }
    }


    public class UnauthorizedResult : ErrorBase
    {
        internal UnauthorizedResult() : base("AuthFailed") { }
    }


    public class ValidationErrorResult : ErrorBase
    {
        internal ValidationErrorResult(IEnumerable<ValidationResult> validationErrors) : base("NotValid")
        {
            ValidationErrors = validationErrors.ToArray();
        }

        public ValidationResult[] ValidationErrors { get; }
    }
}
