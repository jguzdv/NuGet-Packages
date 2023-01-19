using System.ComponentModel.DataAnnotations;

namespace JGUZDV.CQRS.Queries
{
    public class ValidationErrorResult<T> : ErrorBase<T>
    {
        internal ValidationErrorResult(IEnumerable<ValidationResult> validationErrors) : base("NotValid")
        {
            ValidationErrors = validationErrors.ToArray();
        }

        public ValidationResult[] ValidationErrors { get; }
    }
}
