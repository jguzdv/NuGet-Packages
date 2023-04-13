using JGUZDV.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Mvc
{
    public static class QueryHandlerExtensions
    {
        public static async Task<ActionResult<TResult>> ExecuteAsync<TQuery, TResult>(this IQueryHandler<TQuery> queryHandler, TQuery query, ControllerBase controller, IStringLocalizer? loc = null)
            where TQuery : IQuery<TResult>
        {
            await queryHandler.ExecuteAsync(query, controller.User, controller.HttpContext.RequestAborted);
            return query.Result.ToActionResult(loc);
        }
    }
}
