using System.ComponentModel.DataAnnotations;

namespace JGUZDV.CQRS.Commands
{
    public class ValidationErrorResult : ErrorBase
    {
        internal ValidationErrorResult(IEnumerable<ValidationResult> validationErrors) : base("NotValid")
        {
            ValidationErrors = validationErrors.ToArray();
        }

        public ValidationResult[] ValidationErrors { get; }
    }
}
