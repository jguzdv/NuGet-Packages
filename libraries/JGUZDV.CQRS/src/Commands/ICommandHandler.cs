using System.Security.Claims;

namespace JGUZDV.CQRS.Commands;

/// <summary>
/// Describes the interface for a command handler.
/// </summary>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Executes the command for the given principal.
    /// </summary>
    Task<HandlerResult> ExecuteAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct);
}
