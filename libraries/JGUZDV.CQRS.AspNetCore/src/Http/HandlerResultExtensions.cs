using JGUZDV.CQRS.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Http;

public static class HandlerResultExtensions
{
    public static IResult ToHttpResult(this HandlerResult result, IStringLocalizer? sl = null)
    {
        var response = result switch
        {
            SuccessResult => Results.Ok(),

            GenericErrorResult r => Error(r, sl),
            NotFoundResult => Results.NotFound(),
            UnauthorizedResult => Results.Forbid(),
            ValidationErrorResult r => Invalid(r, sl),
            ConflictResult r => Conflict(r, sl),

            CanceledResult => Results.StatusCode(499), //Nginx: "Client Closed Request",

            ErrorBase r => Error(r, sl),
            HandlerResult r => Generic(r)
        };

        return response;
    }

    public static IResult ToHttpResult<T>(this HandlerResult result, Func<CreatedResult, (string Uri, T Obj)> func, IStringLocalizer? sl = null)
    {
        if (result is CreatedResult r)
        {
            var funcResult = func(r);
            return Results.Created(funcResult.Uri, funcResult.Obj); // mir it an der stelle nichts besseres eingefallen
        }

        return result.ToHttpResult(sl);
    }

    private static IResult Generic(HandlerResult r)
    {
        if (r.IsSuccess)
            return Results.Ok();
        else
            return Results.StatusCode(500);
    }


    private static IResult Error(ErrorBase r, IStringLocalizer? sl)
    {
        return Results.Problem(FromFailureCode(r.FailureCode, sl));
    }


    private static readonly string[] NoMemberNames = new[] { "" };
    private static IResult Invalid(ValidationErrorResult r, IStringLocalizer? sl)
    {
        static List<string> GetOrCreate(Dictionary<string, List<string>> dictionary, string key)
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


    private static IResult Conflict(ConflictResult r, IStringLocalizer? sl)
    {
        return Results.Conflict(FromFailureCode(r.FailureCode, sl));
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