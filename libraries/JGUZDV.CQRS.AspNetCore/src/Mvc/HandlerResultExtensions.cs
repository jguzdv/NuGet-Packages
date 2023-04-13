using Microsoft.Extensions.Localization;

using AspNetCoreMvc = Microsoft.AspNetCore.Mvc;

namespace JGUZDV.CQRS.AspNetCore.Mvc;

public static class HandlerResultExtensions
{
    public static AspNetCoreMvc.ActionResult ToActionResult(this HandlerResult result, IStringLocalizer? sl = null)
    {
        var response = result switch
        {
            SuccessResult => new AspNetCoreMvc.OkResult(),

            GenericErrorResult r => Error(r, sl),
            NotFoundResult => new AspNetCoreMvc.NotFoundResult(),
            UnauthorizedResult => new AspNetCoreMvc.ForbidResult(),
            ValidationErrorResult r => Invalid(r, sl),
            ConflictResult r => Conflict(r, sl),

            CanceledResult => new AspNetCoreMvc.StatusCodeResult(499), //Nginx: "Client Closed Request",

            ErrorBase r => Error(r, sl),
            HandlerResult r => Generic(r)
        };

        return response;
    }



    private static AspNetCoreMvc.ActionResult Generic(HandlerResult r)
    {
        if (r.IsSuccess)
            return new AspNetCoreMvc.OkResult();
        else
            return new AspNetCoreMvc.StatusCodeResult(500);
    }


    private static AspNetCoreMvc.ActionResult Error(ErrorBase r, IStringLocalizer? sl)
    {
        return new AspNetCoreMvc.ObjectResult(FromFailureCode(r.FailureCode, sl))
        {
            StatusCode = 500
        };
    }


    private static readonly string[] NoMemberNames = new[] { "" };
    private static AspNetCoreMvc.ActionResult Invalid(ValidationErrorResult r, IStringLocalizer? sl)
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

        var validationProblems = new AspNetCoreMvc.ValidationProblemDetails(errors.ToDictionary(x => x.Key, x => x.Value.ToArray()));
        return new AspNetCoreMvc.BadRequestObjectResult(validationProblems);
    }


    private static AspNetCoreMvc.ActionResult Conflict(ConflictResult r, IStringLocalizer? sl)
    {
        return new AspNetCoreMvc.ConflictObjectResult(FromFailureCode(r.FailureCode, sl));
    }


    private static AspNetCoreMvc.ProblemDetails FromFailureCode(string failureCode, IStringLocalizer? sl)
    {
        return new AspNetCoreMvc.ProblemDetails
        {
            Instance = failureCode,
            Detail = sl?[failureCode] ?? failureCode
        };
    }
}
