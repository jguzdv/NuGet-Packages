using JGUZDV.CQRS.Queries.Results;
using System.Security.Claims;

namespace JGUZDV.CQRS.Queries;

public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
{
    Task<QueryResult<TResult>> ExecuteAsync(TQuery query, ClaimsPrincipal? principal, CancellationToken ct);
}