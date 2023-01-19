using JGUZDV.CQRS.Queries;
using JGUZDV.CQRS.Queries.Results;
using Microsoft.Extensions.Localization;
using Mvc = Microsoft.AspNetCore.Mvc;

namespace JGUZDV.CQRS.AspNetCore;

public static class QueryResultExtensions
{
    public static Mvc.ActionResult<T> ToActionResult<T>(this QueryResult<T> result, IStringLocalizer? sl = null)
    {
        var response = result switch
        {
            GenericErrorResult<T> r => Error(r, sl),
            NotFoundResult<T> => new Mvc.NotFoundResult(),
            UnauthorizedResult<T> => new Mvc.ForbidResult(),
            ValidationErrorResult<T> r => Invalid(r, sl),

            CanceledResult<T> => new Mvc.StatusCodeResult(499), //Nginx: "Client Closed Request",

            ErrorBase<T> r => Error(r, sl),
            QueryResult<T> r => Generic(r)
        };

        return response;
    }



    private static Mvc.ActionResult<T> Generic<T>(QueryResult<T> result)
    {
        if(result.HasResult)
            return new Mvc.OkObjectResult(result.Result);
        else
            return new Mvc.StatusCodeResult(500);
    }


    private static Mvc.ActionResult<T> Error<T>(ErrorBase<T> r, IStringLocalizer? sl)
    {
        return new Mvc.ObjectResult(FromFailureCode(r.FailureCode, sl))
        {
            StatusCode = 500
        };
    }


    private static string[] NoMemberNames = new[] { "" };
    private static Mvc.ActionResult<T> Invalid<T>(ValidationErrorResult<T> r, IStringLocalizer? sl)
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

        var validationProblems = new Mvc.ValidationProblemDetails(errors.ToDictionary(x => x.Key, x => x.Value.ToArray()));
        return new Mvc.BadRequestObjectResult(validationProblems);
    }


    private static Mvc.ProblemDetails FromFailureCode(string failureCode, IStringLocalizer? sl)
    {
        return new Mvc.ProblemDetails
        {
            Instance = failureCode,
            Detail = sl?[failureCode] ?? failureCode
        };
    }
}
