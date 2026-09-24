namespace JGUZDV.Outbox;

/// <summary>
/// Represents the base exception class for all exceptions related to the Outbox library.
/// </summary>
public abstract class OutboxException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxException"/> class with a specified error message.
    /// </summary>
    protected OutboxException(string? message) 
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    protected OutboxException(string? message, Exception? innerException) 
        : base(message, innerException)
    { }
}
