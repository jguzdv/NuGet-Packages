using JGUZDV.CQRS.Commands;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Http
{
    public static class CommandHandlerExtensions
    {
        public static async Task<IResult> ExecuteCommandAsHttpResult<TCommand>(
            this ICommandHandler<TCommand> commandHandler, TCommand command, HttpContext httpContext)
            where TCommand : ICommand
        {
            var stringLocalizer = httpContext.RequestServices.GetService<IStringLocalizer>();
            await commandHandler.ExecuteAsync(command, httpContext.User, httpContext.RequestAborted);
            return HandlerResult.Indeterminate().ToHttpResult(stringLocalizer);
        }
    }
}
