using System.Security.Claims;

namespace JGUZDV.CQRS.Commands;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<CommandResult> ExecuteAsync(TCommand command, CancellationToken ct)
        => ExecuteAsync(command, ClaimsPrincipal.Current, ct);

    Task<CommandResult> ExecuteAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct);
}
