using System.Security.Claims;

namespace JGUZDV.CQRS.Queries;

/// <summary>
/// Describes the interface for a query handler.
/// </summary>
public interface IQueryHandler<in TQuery>
    where TQuery : IQuery
{
    /// <summary>
    /// Execute the query for a given principal.
    /// </summary>
    Task ExecuteAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct);
    
    internal Task ExecuteAsync(object query, ClaimsPrincipal? principal, CancellationToken ct)
        => ExecuteAsync((TQuery)query, principal, ct);
}