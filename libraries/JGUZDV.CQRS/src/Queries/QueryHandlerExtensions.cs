using System.Security.Claims;

namespace JGUZDV.CQRS.Queries
{
    public static class QueryHandlerExtensions
    {
        /// <summary>
        /// Executes the given query and returns the result.
        /// </summary>
        public static async Task<QueryResult<TValue>?> ExecuteQuery<TQuery, TValue>(
            this IQueryHandler<TQuery> queryHandler, IQuery<TValue> query, ClaimsPrincipal? principal, CancellationToken ct)
            where TQuery : IQuery<TValue>
        {
            await queryHandler.ExecuteAsync(query, principal, ct);
            return query.Result;
        }
    }
}
