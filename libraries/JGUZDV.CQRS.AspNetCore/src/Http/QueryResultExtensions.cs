using JGUZDV.CQRS.Queries;
using JGUZDV.CQRS.Queries.Results;
using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Http;

namespace JGUZDV.CQRS.AspNetCore.Http;

public static class QueryResultExtensions
{
    public static IResult ToHttpResult<T>(this QueryResult<T> result, IStringLocalizer? sl = null)
    {
        var response = result switch
        {
            GenericErrorResult<T> r => Error(r, sl),
            NotFoundResult<T> => Results.NotFound(),
            UnauthorizedResult<T> => Results.Forbid(),
            ValidationErrorResult<T> r => Invalid(r, sl),

            CanceledResult<T> => Results.StatusCode(499), //Nginx: "Client Closed Request",

            ErrorBase<T> r => Error(r, sl),
            QueryResult<T> r => Generic(r)
        };

        return response;
    }



    private static IResult Generic<T>(QueryResult<T> result)
    {
        if (result.HasResult)
            return Results.Ok(result.Result);
        else
            return Results.StatusCode(500);
    }


    private static IResult Error<T>(ErrorBase<T> r, IStringLocalizer? sl)
    {
        return Results.Problem(FromFailureCode(r.FailureCode, sl));
    }


    private static string[] NoMemberNames = new[] { "" };
    private static IResult Invalid<T>(ValidationErrorResult<T> r, IStringLocalizer? sl)
    {
        List<string> GetOrCreate(Dictionary<string, List<string>> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            return dictionary[key] = new();
        }

        var errors = new Dictionary<string, List<string>>();
        foreach (var validationError in r.ValidationErrors)
        {
            if (string.IsNullOrWhiteSpace(validationError.ErrorMessage))
                continue;

            var members = validationError.MemberNames.Any()
                ? validationError.MemberNames
                : NoMemberNames;


            foreach (var member in members)
            {
                var memberErrors = GetOrCreate(errors, member);
                if (sl != null)
                    memberErrors.Add(sl[validationError.ErrorMessage]);
                else
                    memberErrors.Add(validationError.ErrorMessage);
            }
        }

        return Results.ValidationProblem(errors.ToDictionary(x => x.Key, x => x.Value.ToArray()));
    }


    private static Microsoft.AspNetCore.Mvc.ProblemDetails FromFailureCode(string failureCode, IStringLocalizer? sl)
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Instance = failureCode,
            Detail = sl?[failureCode] ?? failureCode
        };
    }
}