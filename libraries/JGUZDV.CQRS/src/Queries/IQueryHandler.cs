using System.Security.Claims;

namespace JGUZDV.CQRS.Queries;

public interface IQueryHandler<in TQuery>
        where TQuery : IQuery
{
    Task ExecuteAsync(TQuery query)
        => ExecuteAsync(query, ClaimsPrincipal.Current, default);

    Task ExecuteAsync(TQuery query, ClaimsPrincipal principal)
        => ExecuteAsync(query, principal, default);

    Task ExecuteAsync(TQuery query, CancellationToken ct)
        => ExecuteAsync(query, ClaimsPrincipal.Current, ct);

    Task ExecuteAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct);
    
    internal Task ExecuteAsync(object query, ClaimsPrincipal? principal, CancellationToken ct)
        => ExecuteAsync((TQuery)query, principal, ct);
}