using JGUZDV.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Mvc
{
    public static class QueryHandlerWebExtensions
    {
        public static Task<ActionResult<TResult>> ExecuteQueryAsync<TQuery, TResult>(this ControllerBase controller, IQueryHandler<TQuery, TResult> queryHandler, TQuery query, IStringLocalizer? loc = null)
            where TQuery : IQuery<TResult>
            => ExecuteQueryHandler(queryHandler, query, controller, loc);

        public static Task<ActionResult<TResult>> ExecuteAsync<TQuery, TResult>(this IQueryHandler<TQuery, TResult> queryHandler, TQuery query, ControllerBase controller, IStringLocalizer? loc = null)
            where TQuery : IQuery<TResult>
            => ExecuteQueryHandler(queryHandler, query, controller, loc);

        private static async Task<ActionResult<TResult>> ExecuteQueryHandler<TQuery, TResult>(IQueryHandler<TQuery, TResult> queryHandler, TQuery query, ControllerBase controller, IStringLocalizer? loc)
            where TQuery : IQuery<TResult>
        {
            var queryResult = await queryHandler.ExecuteAsync(query, controller.User, controller.HttpContext.RequestAborted);
            return queryResult.ToActionResult(loc);
        }
    }
}
