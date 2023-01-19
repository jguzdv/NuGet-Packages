using JGUZDV.CQRS.Commands;
using Microsoft.Extensions.Localization;
using Mvc = Microsoft.AspNetCore.Mvc;

namespace JGUZDV.CQRS.AspNetCore
{
    public static class CommandHandlerWebExtensions
    {
        public static Task<Mvc.IActionResult> ExecuteCommandAsync<TCommand>(this Mvc.ControllerBase controller, ICommandHandler<TCommand> commandHandler, TCommand command, IStringLocalizer? loc = null)
            where TCommand : ICommand
            => ExecuteCommandHandler(commandHandler, command, controller, loc);

        public static Task<Mvc.IActionResult> ExecuteAsync<TCommand>(this ICommandHandler<TCommand> commandHandler, TCommand command, Mvc.ControllerBase controller, IStringLocalizer? loc = null)
            where TCommand : ICommand
            => ExecuteCommandHandler(commandHandler, command, controller, loc);

        private static async Task<Mvc.IActionResult> ExecuteCommandHandler<TCommand>(ICommandHandler<TCommand> commandHandler, TCommand command, Mvc.ControllerBase controller, IStringLocalizer? loc) 
            where TCommand : ICommand
        {
            var commandResult = await commandHandler.ExecuteAsync(command, controller.User, controller.HttpContext.RequestAborted);
            return commandResult.ToActionResult(loc);
        }
    }
}
