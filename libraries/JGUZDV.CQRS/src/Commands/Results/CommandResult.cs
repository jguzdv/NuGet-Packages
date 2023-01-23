using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JGUZDV.CQRS.Commands
{
    public abstract class CommandResult
    {
        private static readonly SuccessResult _genericSuccess = new SuccessResult();

        protected CommandResult() { }

        public bool IsSuccess { get; protected set; } = false;

        public string[]? WarningCodes { get; init; }


        /// <summary>
        /// Use this to indicate the command completed successfully.
        /// </summary>
        public static SuccessResult Success()
            => _genericSuccess;

        /// <summary>
        /// Use this to indicate the command completed successfully with warnings.
        /// </summary>
        public static SuccessResult Success(params string[] warnings)
            => new SuccessResult { WarningCodes = warnings };

        /// <summary>
        /// Use this to indicate the command created something.
        /// </summary>
        public static CreatedResult Created(object createdId, params string[]? warnings)
            => new CreatedResult(createdId) { WarningCodes = warnings };

        /// <summary>
        /// Use this to indicate the command failed generally (though an unhandled exception)
        /// </summary>
        public static GenericErrorResult Fail(string reason)
            => new GenericErrorResult(reason);

        /// <summary>
        /// Use this to indicate the command failed generally (though an unhandled exception)
        /// </summary>
        public static GenericErrorResult Error(string reason)
            => new GenericErrorResult(reason);

        /// <summary>
        /// Use this to indicate the command was not vaild (invalid data passed [HTTP 400])
        /// </summary>
        public static ValidationErrorResult NotValid(IEnumerable<ValidationResult> validationErrors) =>
            new ValidationErrorResult(validationErrors);

        /// <summary>
        /// Use this to indicate the command was not vaild (invalid data passed [HTTP 400])
        /// </summary>
        public static ValidationErrorResult NotValid(params string[] reasons) =>
            new ValidationErrorResult(reasons.Select(x => new ValidationResult(x)).ToArray());

        /// <summary>
        /// Use this to indicate the command had a conflict (could not execute due to domain-state [HTTP 400])
        /// </summary>
        public static ConflictResult Conflict(string reason) =>
            new ConflictResult(reason);

        /// <summary>
        /// Use this to indicate the command was not allowed (user may not do this [HTTP 403])
        /// </summary>
        public static UnauthorizedResult NotAllowed()
            => new UnauthorizedResult();

        /// <summary>
        /// Use this to indicate the command could not find something (wrong ids and similar [HTTP 404])
        /// </summary>
        public static NotFoundResult NotFound()
            => new NotFoundResult();

        public static CanceledResult Canceled(CancellationToken ct = default)
            => new CanceledResult(ct);

        public static implicit operator bool(CommandResult result) => result.IsSuccess;
    }
}
