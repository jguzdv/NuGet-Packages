using System.ComponentModel.DataAnnotations;

namespace JGUZDV.CQRS
{
    public abstract class HandlerResult
    {
        /// <summary>
        /// Indicates, if the handler was successfull or not
        /// </summary>
        public bool IsSuccess { get; protected set; } = false;
        
        /// <summary>
        /// Contains warning codes, if the handler issued any warnings about the current process.
        /// Should only be set in success or partial success cases.
        /// </summary>
        public string[]? WarningCodes { get; init; }
        public bool HasWarnings => WarningCodes?.Any() == true;


        public static implicit operator bool(HandlerResult result) => result.IsSuccess;



        /// <summary>
        /// Use this to indicate the command completed successfully.
        /// </summary>
        public static SuccessResult Success() => SuccessResult.GenericSuccess;

        /// <summary>
        /// Use this to indicate the command completed successfully with warnings.
        /// </summary>
        public static SuccessResult Success(params string[] warnings)
            => new() { WarningCodes = warnings };


        /// <summary>
        /// Use this to indicate the command created something.
        /// </summary>
        public static CreatedResult Created(params string[]? warnings) => new() { WarningCodes = warnings };


        /// <summary>
        /// Use this to indicate the command created something.
        /// </summary>
        public static CreatedResult<T> Created<T>(T value, params string[]? warnings)
            => new CreatedResult<T>(value) { WarningCodes = warnings };


        /// <summary>
        /// Use this to indicate the handler failed generally (though an unhandled exception)
        /// </summary>
        public static GenericErrorResult Fail(string reason) => Error(reason);

        /// <summary>
        /// Use this to indicate the handler failed generally (though an unhandled exception)
        /// </summary>
        public static GenericErrorResult Error(string reason) => new(reason);

        /// <summary>
        /// Use this to indicate the request was not vaild (invalid data passed [HTTP 400])
        /// </summary>
        public static ValidationErrorResult NotValid(IEnumerable<ValidationResult> validationErrors) => new(validationErrors);

        /// <summary>
        /// Use this to indicate the request was not vaild (invalid data passed [HTTP 400])
        /// </summary>
        public static ValidationErrorResult NotValid(params string[] reasons) =>
            new(reasons.Select(x => new ValidationResult(x)).ToArray());

        /// <summary>
        /// Use this to indicate the handler had a conflict (could not execute due to domain-state [HTTP 400])
        /// </summary>
        public static ConflictResult Conflict(string reason) => new(reason);

        /// <summary>
        /// Use this to indicate the handler was not allowed to execute (user may not do this [HTTP 403])
        /// </summary>
        public static UnauthorizedResult NotAllowed() => new();

        /// <summary>
        /// Use this to indicate the handler could not find something (wrong ids and similar [HTTP 404])
        /// </summary>
        public static NotFoundResult NotFound() => new();

        public static CanceledResult Canceled(CancellationToken ct = default) => new(ct);
    }
}
