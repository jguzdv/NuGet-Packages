using JGUZDV.CQRS.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace JGUZDV.CQRS.AspNetCore.Mvc
{
    public static class CommandHandlerWebExtensions
    {
        public static Task<IActionResult> ExecuteCommandAsync<TCommand>(this ControllerBase controller, ICommandHandler<TCommand> commandHandler, TCommand command, IStringLocalizer? loc = null)
            where TCommand : ICommand
            => ExecuteCommandHandler(commandHandler, command, controller, loc);

        public static Task<IActionResult> ExecuteAsync<TCommand>(this ICommandHandler<TCommand> commandHandler, TCommand command, ControllerBase controller, IStringLocalizer? loc = null)
            where TCommand : ICommand
            => ExecuteCommandHandler(commandHandler, command, controller, loc);

        private static async Task<IActionResult> ExecuteCommandHandler<TCommand>(ICommandHandler<TCommand> commandHandler, TCommand command, ControllerBase controller, IStringLocalizer? loc)
            where TCommand : ICommand
        {
            var commandResult = await commandHandler.ExecuteAsync(command, controller.User, controller.HttpContext.RequestAborted);
            return commandResult.ToActionResult(loc);
        }
    }
}
