using JGUZDV.CQRS.Commands;
using Microsoft.Extensions.Localization;
using Mvc = Microsoft.AspNetCore.Mvc;

namespace JGUZDV.CQRS.AspNetCore;

public static class CommandResultExtensions
{
    public static Mvc.ActionResult ToActionResult(this CommandResult result, IStringLocalizer? sl = null)
    {
        var response = result switch
        {
            SuccessResult => new Mvc.OkResult(),
            CreatedResult r => Created(r),

            GenericErrorResult r => Error(r, sl),
            NotFoundResult => new Mvc.NotFoundResult(),
            UnauthorizedResult => new Mvc.ForbidResult(),
            ValidationErrorResult r => Invalid(r, sl),
            ConflictResult r => Conflict(r, sl),

            CanceledResult => new Mvc.StatusCodeResult(499), //Nginx: "Client Closed Request",

            ErrorBase r => Error(r, sl),
            CommandResult r => Generic(r)
        };

        return response;
    }



    private static Mvc.ActionResult Generic(CommandResult r)
    {
        if (r.IsSuccess)
            return new Mvc.OkResult();
        else
            return new Mvc.StatusCodeResult(500);
    }


    private static Mvc.ActionResult Created(CreatedResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.CreatedAtUrl))
            return new Mvc.CreatedResult(result.CreatedAtUrl, result);

        return new Mvc.OkObjectResult(result);
    }


    private static Mvc.ActionResult Error(ErrorBase r, IStringLocalizer? sl)
    {
        return new Mvc.ObjectResult(FromFailureCode(r.FailureCode, sl))
        {
            StatusCode = 500
        };
    }


    private static string[] NoMemberNames = new[] { "" };
    private static Mvc.ActionResult Invalid(ValidationErrorResult r, IStringLocalizer? sl)
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


    private static Mvc.ActionResult Conflict(ConflictResult r, IStringLocalizer? sl)
    {
        return new Mvc.ConflictObjectResult(FromFailureCode(r.FailureCode, sl));
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
