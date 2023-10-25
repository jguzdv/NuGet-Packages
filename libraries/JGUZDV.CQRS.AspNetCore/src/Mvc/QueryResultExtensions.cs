using JGUZDV.CQRS.Queries;

using Microsoft.Extensions.Localization;

using AspNetCoreMvc = Microsoft.AspNetCore.Mvc;

namespace JGUZDV.CQRS.AspNetCore.Mvc;

public static class QueryResultExtensions
{
    public static AspNetCoreMvc.ActionResult<T> ToActionResult<T>(this QueryResult<T>? result, IStringLocalizer? sl = null)
    {
        if (result == null)
            return HandlerResult.Fail($"{nameof(QueryResult<T>)} was null.").ToActionResult(sl);

        return result.HasValue
                ? new AspNetCoreMvc.OkObjectResult(result.Value)
                : result.HandlerResult.ToActionResult(sl);
    }
}
