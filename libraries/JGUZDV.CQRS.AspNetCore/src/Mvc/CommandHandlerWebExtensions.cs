using JGUZDV.CQRS.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Mvc
{
    public static class CommandHandlerExtensions
    {
        public static async Task<IActionResult> ExecuteAsync<TCommand>(this ICommandHandler<TCommand> commandHandler, TCommand command, ControllerBase controller, IStringLocalizer? loc = null)
            where TCommand : ICommand
        {
            var commandResult = await commandHandler.ExecuteAsync(command, controller.User, controller.HttpContext.RequestAborted);
            return commandResult.ToActionResult(loc);
        }
    }
}
