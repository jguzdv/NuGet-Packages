using JGUZDV.CQRS.Queries;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Mvc
{
    /// <summary>
    /// Extension methods for query handlers.
    /// </summary>
    public static class QueryHandlerExtensions
    {
        /// <summary>
        /// Executes the query and returns the result as an action result.
        /// The method will use the current controller to get the user and the request cancellation token.
        /// </summary>
        public static async Task<ActionResult<TResult>> ExecuteAsync<TQuery, TResult>(this IQueryHandler<TQuery> queryHandler, TQuery query, ControllerBase controller, IStringLocalizer? loc = null)
            where TQuery : IQuery<TResult>
        {
            await queryHandler.ExecuteAsync(query, controller.User, controller.HttpContext.RequestAborted);
            return query.Result.ToActionResult(loc);
        }
    }
}
