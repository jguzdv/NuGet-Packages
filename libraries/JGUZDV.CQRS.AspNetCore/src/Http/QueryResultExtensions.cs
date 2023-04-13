using JGUZDV.CQRS.Queries;
using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Http;

namespace JGUZDV.CQRS.AspNetCore.Http;

public static class QueryResultExtensions
{
    public static IResult ToHttpResult<T>(this QueryResult<T> result, IStringLocalizer? sl = null) 
        => result.HasValue
            ? Results.Ok(result.Value)
            : result.HandlerResult.ToHttpResult(sl);
}