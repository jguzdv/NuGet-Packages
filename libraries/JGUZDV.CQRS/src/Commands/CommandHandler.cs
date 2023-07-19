using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JGUZDV.CQRS.Commands;

public abstract partial class CommandHandler<TCommand, TContext> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public abstract ILogger Logger { get; }

    protected bool SkipAuthorization { get; set; } = false;

    protected abstract Task<TContext> InitializeAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct);

    protected virtual TCommand NormalizeCommand(TCommand command, TContext context, ClaimsPrincipal? principal)
        => command;

    protected virtual Task<bool> AuthorizeAsync(TCommand command, TContext context, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(false);

    protected virtual Task<List<ValidationResult>> ValidateAsync(TCommand command, TContext context, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(new List<ValidationResult>());


    protected abstract Task<HandlerResult> ExecuteInternalAsync(TCommand command, TContext context, ClaimsPrincipal? principal, CancellationToken ct);


    public async Task<HandlerResult> ExecuteAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
    {
        try
        {
            var context = await InitializeAsync(command, principal, ct);
            if (ct.IsCancellationRequested)
            {
                Log.StepCancelled(Logger, nameof(InitializeAsync));
                return HandlerResult.Canceled(ct);
            }

            command = NormalizeCommand(command, context, principal);

            if (!SkipAuthorization)
            {
                var isAuthorized = await AuthorizeAsync(command, context, principal, ct);
                Log.AuthorizationResult(Logger, isAuthorized);

                if (!isAuthorized)
                    return HandlerResult.NotAllowed();

                if (ct.IsCancellationRequested)
                {
                    Log.StepCancelled(Logger, nameof(AuthorizeAsync));
                    return HandlerResult.Canceled(ct);
                }
            }

            var validationResult = await ValidateAsync(command, context, principal, ct);
            if (validationResult.Any())
            {
                var result = HandlerResult.NotValid(validationResult);
                Log.HandlerResult(Logger, result);

                return result;
            }

            Log.ValidationResult(Logger, true);

            if (ct.IsCancellationRequested)
            {
                Log.StepCancelled(Logger, nameof(ValidateAsync));
                return HandlerResult.Canceled(ct);
            }

            return await ExecuteInternalAsync(command, context, principal, ct);
        }
        catch (CommandException ex)
        {
            Log.HandlerResult(Logger, ex.CommandResult);
            return ex.CommandResult;
        }
        catch (TaskCanceledException tcex)
        {
            Log.Cancelled(Logger);
            return HandlerResult.Canceled(tcex.CancellationToken);
        }
        catch (Exception ex)
        {
            Log.ExecutionError(Logger, ex);
            return HandlerResult.Fail("GenericError");
        }
    }

    protected static partial class Log
    {
        [LoggerMessage(4990, LogLevel.Debug, "Command has been cancelled.")]
        internal static partial void Cancelled(ILogger logger);
        
        [LoggerMessage(4991, LogLevel.Debug, "Command has been cancelled after {step}.")]
        internal static partial void StepCancelled(ILogger logger, string step);


        [LoggerMessage(4000, LogLevel.Information, "Command validation result was: {valid}", EventName = "CommandValidation")]
        internal static partial void ValidationResult(ILogger logger, bool valid);

        [LoggerMessage(4001, LogLevel.Debug, "Command validation result for {memberNames}: {message}", EventName = "CommandValidation", SkipEnabledCheck = true)]
        internal static partial void ValidationResultDetail(ILogger logger, string memberNames, string message);

        [LoggerMessage(4002, LogLevel.Information, "Command execution detected a conflict: {conflict}", EventName = "CommandExecution")]
        internal static partial void Conflict(ILogger logger, string conflict);


        [LoggerMessage(4030, LogLevel.Information, "Command authorization result was: {authorized}", EventName = "CommandAuthorization")]
        internal static partial void AuthorizationResult(ILogger logger, bool authorized);
        
        [LoggerMessage(4040, LogLevel.Information, "Command did not find an object to act on (NotFound).", EventName = "CommandExecution")]
        internal static partial void NotFound(ILogger logger);


        [LoggerMessage(5000, LogLevel.Error, "Command execution threw an exception.")]
        internal static partial void ExecutionError(ILogger logger, Exception ex);

        [LoggerMessage(5001, LogLevel.Error, "Command execution reported a generic error: {message}")]
        internal static partial void GenericExecutionError(ILogger logger, string message);


        internal static void HandlerResult(ILogger logger, HandlerResult result) {
            if (result is GenericErrorResult ger)
                GenericExecutionError(logger, ger.FailureCode);
            else if (result is ValidationErrorResult ver)
                ValidationErrorResult(logger, ver);
            else if (result is ConflictResult cfr)
                Conflict(logger, cfr.FailureCode);
            else if (result is UnauthorizedResult)
                AuthorizationResult(logger, false);
            else if (result is NotFoundResult)
                NotFound(logger);
            else if (result is CanceledResult)
                Cancelled(logger);
        }


        internal static void ValidationErrorResult(ILogger logger, ValidationErrorResult result)
        {
            ValidationResult(logger, false);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var v in result.ValidationErrors)
                    ValidationResultDetail(logger, string.Join(", ", v.MemberNames), v.ErrorMessage ?? "n/a");
            }
        }
    }
}


public abstract class CommandHandler<TCommand> : CommandHandler<TCommand, object>
    where TCommand : ICommand
{
    protected sealed override Task<object> InitializeAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(new object());


    protected sealed override Task<HandlerResult> ExecuteInternalAsync(TCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        => ExecuteInternalAsync(command, principal, ct);
    protected abstract Task<HandlerResult> ExecuteInternalAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct);


    protected sealed override TCommand NormalizeCommand(TCommand command, object context, ClaimsPrincipal? principal)
        => NormalizeCommand(command, principal);
    protected virtual TCommand NormalizeCommand(TCommand command, ClaimsPrincipal? principal)
        => command;


    protected sealed override Task<bool> AuthorizeAsync(TCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        => AuthorizeAsync(command, principal, ct);
    protected virtual Task<bool> AuthorizeAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(false);


    protected sealed override Task<List<ValidationResult>> ValidateAsync(TCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        => ValidateAsync(command, principal, ct);
    protected virtual Task<List<ValidationResult>> ValidateAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(new List<ValidationResult>());
}