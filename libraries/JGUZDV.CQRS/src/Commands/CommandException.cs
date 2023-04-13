namespace JGUZDV.CQRS.Commands;

/// <summary>
/// Especially base clases build upon CommandHandler need the possiblity to exceptionally break out of command execution.
/// This class enables that.
/// </summary>
public class CommandException : Exception {
    public CommandException(HandlerResult commandResult)
    {
        ArgumentNullException.ThrowIfNull(commandResult);

        if (commandResult.IsSuccess)
            throw new ArgumentException("Cannot throw exception for CommandResult, that signals success");
        CommandResult = commandResult;
    }

    public HandlerResult CommandResult { get; }
}
