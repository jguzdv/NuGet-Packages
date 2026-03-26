using JGUZDV.CQRS.Commands;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Http
{
    public static class CommandHandlerExtensions
    {
        /// <summary>
        /// Executes the given command and returns the result using the User and CancellationToken from the HttpContext.
        /// </summary>
        public static async Task ExecuteCommand<TCommand>(
            this ICommandHandler<TCommand> commandHandler, TCommand command, HttpContext httpContext)
            where TCommand : ICommand
        {
            await commandHandler.ExecuteAsync(command, httpContext.User, httpContext.RequestAborted);
        }

        /// <summary>
        /// Executes the given command and converts the result to IResult.
        /// A common string localizer will be passed into ToHttpResult if available, so that the result can be localized.
        /// </summary>
        public static async Task<IResult> ExecuteCommandAsHttpResult<TCommand>(
            this ICommandHandler<TCommand> commandHandler, TCommand command, HttpContext httpContext)
            where TCommand : ICommand
        {
            var stringLocalizer = httpContext.RequestServices.GetService<IStringLocalizer>();
            var commandResult = await commandHandler.ExecuteAsync(command, httpContext.User, httpContext.RequestAborted);
            return commandResult.ToHttpResult(stringLocalizer);
        }
    }
}
