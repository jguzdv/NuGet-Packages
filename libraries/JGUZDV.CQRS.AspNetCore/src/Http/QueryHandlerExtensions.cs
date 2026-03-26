using JGUZDV.CQRS.Queries;

using Microsoft.AspNetCore.Http;

namespace JGUZDV.CQRS.AspNetCore.Http
{
    public static class QueryHandlerExtensions
    {
        /// <summary>
        /// Executes the given query and returns the result.
        /// </summary>
        public static async Task<QueryResult<TValue>> ExecuteQuery<TQuery, TValue>(
            this IQueryHandler<TQuery> queryHandler, IQuery<TValue> query, HttpContext httpContext)
            where TQuery : IQuery<TValue>
        {
            await queryHandler.ExecuteQuery(query, httpContext.User, httpContext.RequestAborted);
            return query.Result ?? HandlerResult.Indeterminate();
        }
    }
}
