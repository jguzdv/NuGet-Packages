using JGUZDV.CQRS.Queries;
using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Http;

namespace JGUZDV.CQRS.AspNetCore.Http;

public static class QueryResultExtensions
{
    /// <summary>
    /// Converts a <see cref="QueryResult{T}"/> to a <see cref="IResult"/>.
    /// </summary>
    public static IResult ToHttpResult<T>(this QueryResult<T>? result, IStringLocalizer? sl = null)
    {
        if (result == null)
            return HandlerResult.Fail($"{nameof(QueryResult<T>)} was null.").ToHttpResult(sl);

        return result.HasValue
            ? Results.Ok(result.Value)
            : result.HandlerResult.ToHttpResult(sl);
    }
}