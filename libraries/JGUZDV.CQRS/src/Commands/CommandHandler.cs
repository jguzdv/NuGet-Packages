using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace JGUZDV.CQRS.Commands;

/// <summary>
/// Handles a command of type TCommand and provides a context of type TContext for the execution steps of the command.
/// </summary>
public abstract partial class CommandHandler<TCommand, TContext> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// A logger for this command handler. This should be implemented by the inheriting class to provide a logger instance, e.g. via dependency injection.
    /// </summary>
    public abstract ILogger Logger { get; }

    /// <summary>
    /// If set to true, authorization will be skipped.
    /// </summary>
    protected bool SkipAuthorization { get; set; } = false;

    /// <summary>
    /// Initializes the command context.
    /// Throw CommandExceptions with appropriate CommandResults to return specific results from the command handler, e.g. NotFound, without executing the command.
    /// </summary>
    protected abstract Task<TContext> InitializeAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct);

    /// <summary>
    /// Normalizes the command object before anything else runs. Can be used for trimming or default values.
    /// </summary>
    protected virtual TCommand NormalizeCommand(TCommand command, TContext context, ClaimsPrincipal? principal)
        => command;

    /// <summary>
    /// Authorizes the usage of the command.
    /// If this is not overriden or explicitly skipped, the command will fail with a NotAllowed result.
    /// </summary>
    protected virtual Task<bool> AuthorizeAsync(TCommand command, TContext context, ClaimsPrincipal? principal, CancellationToken ct)
    {
        Log.AuthorizeNotImplemented(Logger);
        return Task.FromResult(false);
    }

    /// <summary>
    /// Validates the command object before execution.
    /// If this is not overriden, the command will be considered valid by default.
    /// </summary>
    protected virtual Task<List<ValidationResult>> ValidateAsync(TCommand command, TContext context, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(new List<ValidationResult>());

    /// <summary>
    /// Executes the specified command asynchronously within the given context and returns the result of the operation.
    /// </summary>
    /// <remarks>Override this method in a derived class to implement custom command execution logic. The
    /// result may depend on the provided context and principal.</remarks>
    protected abstract Task<HandlerResult> ExecuteInternalAsync(TCommand command, TContext context, ClaimsPrincipal? principal, CancellationToken ct);

    /// <summary>
    /// Executes the default command pipeline.
    /// </summary>
    public async Task<HandlerResult> ExecuteAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
    {
        TContext? context = default;
        try
        {
            context = await InitializeAsync(command, principal, ct);
            if (ct.IsCancellationRequested)
            {
                Log.StepCancelled(Logger, nameof(InitializeAsync));
                return HandlerResult.Canceled(ct);
            }

            command = NormalizeCommand(command, context, principal);

            if (!SkipAuthorization)
            {
                var isAuthorized = await AuthorizeAsync(command, context, principal, ct);
                Log.AuthorizationResult(Logger, isAuthorized? LogLevel.Debug : LogLevel.Information, isAuthorized);

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

            Log.ValidationResult(Logger, LogLevel.Debug, true);

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
        finally
        {
            if(context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Provides Logging methods for the CommandHandler.
    /// </summary>
    protected static partial class Log
    {
        [LoggerMessage(4990, LogLevel.Debug, "Command has been cancelled.")]
        internal static partial void Cancelled(ILogger logger);
        
        [LoggerMessage(4991, LogLevel.Debug, "Command has been cancelled after {step}.")]
        internal static partial void StepCancelled(ILogger logger, string step);


        [LoggerMessage(EventId = 4000, Message = "Command validation result was: {valid}", EventName = "CommandValidation")]
        internal static partial void ValidationResult(ILogger logger, LogLevel logLevel, bool valid);

        [LoggerMessage(4001, LogLevel.Debug, "Command validation result for {memberNames}: {message}", EventName = "CommandValidationDetail", SkipEnabledCheck = true)]
        internal static partial void ValidationResultDetail(ILogger logger, string memberNames, string message);

        [LoggerMessage(4002, LogLevel.Information, "Command execution detected a conflict: {conflict}", EventName = "CommandExecution")]
        internal static partial void Conflict(ILogger logger, string conflict);


        [LoggerMessage(EventId = 4030, Message = "Command authorization result was: {authorized}", EventName = "CommandAuthorization")]
        internal static partial void AuthorizationResult(ILogger logger, LogLevel loglevel, bool authorized);


        [LoggerMessage(4031, LogLevel.Warning, "Command authorization failed due to method not being overriden.", EventName = "CommandAuthorizationMissing")]
        internal static partial void AuthorizeNotImplemented(ILogger logger);

        [LoggerMessage(4040, LogLevel.Information, "Command did not find an object to act on (NotFound).", EventName = "CommandExecutionObjectNotFound")]
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
                AuthorizationResult(logger, LogLevel.Information, false);
            else if (result is NotFoundResult)
                NotFound(logger);
            else if (result is CanceledResult)
                Cancelled(logger);
        }


        internal static void ValidationErrorResult(ILogger logger, ValidationErrorResult result)
        {
            ValidationResult(logger, LogLevel.Information, false);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                foreach (var v in result.ValidationErrors)
                    ValidationResultDetail(logger, string.Join(", ", v.MemberNames), v.ErrorMessage ?? "n/a");
            }
        }
    }
}

/// <summary>
/// A context-less command handler that simply uses object as context and ignores it. 
/// This can be used for simple commands that don't require a context, to avoid the boilerplate of providing a context type and returning a dummy context object in the InitializeAsync method.
/// </summary>
public abstract class CommandHandler<TCommand> : CommandHandler<TCommand, object>
    where TCommand : ICommand
{
    /// <summary>
    /// Overriden and sealed to provide a dummy context object for the command execution pipeline. The context is not used in this implementation, so it simply returns a new object instance.
    /// </summary>
    protected sealed override Task<object> InitializeAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(new object());


    /// <summary>
    /// Executes the specified command asynchronously within the given context and returns the result of the operation.
    /// </summary>
    protected sealed override Task<HandlerResult> ExecuteInternalAsync(TCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        => ExecuteInternalAsync(command, principal, ct);

    /// <summary>
    /// Executes the specified command asynchronously within the given context and returns the result of the operation.
    /// </summary>
    protected abstract Task<HandlerResult> ExecuteInternalAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct);



    /// <summary>
    /// Normalizes the command object before anything else runs. Can be used for trimming or default values.
    /// </summary>
    protected sealed override TCommand NormalizeCommand(TCommand command, object context, ClaimsPrincipal? principal)
        => NormalizeCommand(command, principal);

    /// <summary>
    /// Normalizes the command object before anything else runs. Can be used for trimming or default values.
    /// </summary>
    protected virtual TCommand NormalizeCommand(TCommand command, ClaimsPrincipal? principal)
        => command;

    /// <summary>
    /// Authorizes the usage of the command.
    /// If this is not overriden or explicitly skipped, the command will fail with a NotAllowed result.
    /// </summary>
    protected sealed override Task<bool> AuthorizeAsync(TCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        => AuthorizeAsync(command, principal, ct);

    /// <summary>
    /// Authorizes the usage of the command.
    /// If this is not overriden or explicitly skipped, the command will fail with a NotAllowed result.
    /// </summary>
    protected virtual Task<bool> AuthorizeAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(false);


    /// <summary>
    /// Validates the command object before execution.
    /// If this is not overriden, the command will be considered valid by default.
    /// </summary>
    protected sealed override Task<List<ValidationResult>> ValidateAsync(TCommand command, object context, ClaimsPrincipal? principal, CancellationToken ct)
        => ValidateAsync(command, principal, ct);

    /// <summary>
    /// Validates the command object before execution.
    /// If this is not overriden, the command will be considered valid by default.
    /// </summary>
    protected virtual Task<List<ValidationResult>> ValidateAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct)
        => Task.FromResult(new List<ValidationResult>());
}