using System.Security.Claims;

namespace JGUZDV.CQRS.Commands;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<HandlerResult> ExecuteAsync(TCommand command, CancellationToken ct)
        => ExecuteAsync(command, ClaimsPrincipal.Current, ct);

    Task<HandlerResult> ExecuteAsync(TCommand command, ClaimsPrincipal? principal, CancellationToken ct);
}
