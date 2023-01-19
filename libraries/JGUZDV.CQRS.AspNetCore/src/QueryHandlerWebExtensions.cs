using JGUZDV.CQRS.Queries;
using Microsoft.Extensions.Localization;
using Mvc = Microsoft.AspNetCore.Mvc;

namespace JGUZDV.CQRS.AspNetCore
{
    public static class QueryHandlerWebExtensions
    {
        public static Task<Mvc.ActionResult<TResult>> ExecuteQueryAsync<TQuery, TResult>(this Mvc.ControllerBase controller, IQueryHandler<TQuery, TResult> queryHandler, TQuery query, IStringLocalizer? loc = null)
            where TQuery : IQuery<TResult>
            => ExecuteQueryHandler(queryHandler, query, controller, loc);

        public static Task<Mvc.ActionResult<TResult>> ExecuteAsync<TQuery, TResult>(this IQueryHandler<TQuery, TResult> queryHandler, TQuery query, Mvc.ControllerBase controller, IStringLocalizer? loc = null)
            where TQuery : IQuery<TResult>
            => ExecuteQueryHandler(queryHandler, query, controller, loc);

        private static async Task<Mvc.ActionResult<TResult>> ExecuteQueryHandler<TQuery, TResult>(IQueryHandler<TQuery, TResult> queryHandler, TQuery query, Mvc.ControllerBase controller, IStringLocalizer? loc)
            where TQuery : IQuery<TResult>
        {
            var queryResult = await queryHandler.ExecuteAsync(query, controller.User, controller.HttpContext.RequestAborted);
            return queryResult.ToActionResult(loc);
        }
    }
}
