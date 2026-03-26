using JGUZDV.CQRS.Queries;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

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

        /// <summary>
        /// Executes the given query and converts the result to IResult.
        /// A common string localizer will be passed into ToHttpResult if available, so that the result can be localized.
        /// </summary>
        public static async Task<IResult> ExecuteQueryAsHttpResult<TQuery, TValue>(
            this IQueryHandler<TQuery> queryHandler, IQuery<TValue> query, HttpContext httpContext)
            where TQuery : IQuery<TValue>
        {
            var stringLocalizer = httpContext.RequestServices.GetService<IStringLocalizer>();

            await queryHandler.ExecuteQuery(query, httpContext.User, httpContext.RequestAborted);
            return (query.Result ?? HandlerResult.Indeterminate()).ToHttpResult(stringLocalizer);
        }
    }
}
