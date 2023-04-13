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
                // TODO: Log Cancellation?
                return HandlerResult.Canceled(ct);

            command = NormalizeCommand(command, context, principal);

            if (!SkipAuthorization)
            {
                var isAuthorized = await AuthorizeAsync(command, context, principal, ct);
                if (!isAuthorized)
                    // TODO: Log Authorization Result as Information
                    return HandlerResult.NotAllowed();

                if (ct.IsCancellationRequested)
                    // TODO: Log Cancellation?
                    return HandlerResult.Canceled(ct);
            }

            var validationResult = await ValidateAsync(command, context, principal, ct);
            if (validationResult.Any())
                // TODO: Log Valiation Result as Debug
                return HandlerResult.NotValid(validationResult);

            if (ct.IsCancellationRequested)
                // TODO: Log Cancellation?
                return HandlerResult.Canceled(ct);

            return await ExecuteInternalAsync(command, context, principal, ct);
        }
        catch (CommandException ex)
        {
            return ex.CommandResult;
        }
        catch (TaskCanceledException)
        {
            // TODO: Log Exception?
            return HandlerResult.Canceled();
        }
        catch (Exception ex)
        {
            // TODO: Log Exception
            return HandlerResult.Fail("GenericError");
        }
    }

    protected static partial class Log
    {
        [LoggerMessage(1, LogLevel.Debug, "Command {param1} has not been valid.")]
        internal static partial void Invalid(ILogger logger, object param1);
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