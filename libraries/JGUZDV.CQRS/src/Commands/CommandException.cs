namespace JGUZDV.CQRS.Commands;

/// <summary>
/// Especially base clases build upon CommandHandler need the possiblity to exceptionally break out of command execution.
/// This class enables that.
/// </summary>
public class CommandException : Exception {

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandException"/> class with the specified <see cref="HandlerResult"/>.
    /// </summary>
    public CommandException(HandlerResult commandResult)
    {
        ArgumentNullException.ThrowIfNull(commandResult);

        if (commandResult.IsSuccess)
            throw new ArgumentException("Cannot throw exception for CommandResult, that signals success");
        CommandResult = commandResult;
    }

    /// <summary>
    /// The <see cref="HandlerResult"/> that can be used to return from the command execution.
    /// </summary>
    public HandlerResult CommandResult { get; }
}
