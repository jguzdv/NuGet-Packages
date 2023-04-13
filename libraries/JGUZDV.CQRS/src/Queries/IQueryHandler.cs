using System.Security.Claims;

namespace JGUZDV.CQRS.Queries;

public interface IQueryHandler<in TQuery>
        where TQuery : IQuery
{
    Task ExecuteAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct);
    internal Task ExecuteAsync(object query, ClaimsPrincipal? principal, CancellationToken ct)
        => ExecuteAsync((TQuery)query, principal, ct);
}