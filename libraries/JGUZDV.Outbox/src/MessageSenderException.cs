namespace JGUZDV.Outbox;

/// <summary>
/// Represents an exception that occurs when sending a message fails.
/// </summary>
public class MessageSenderException : OutboxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSenderException"/> class with a specified error message.
    /// </summary>
    public MessageSenderException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSenderException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    public MessageSenderException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>
    /// Gets or sets a value indicating whether the exception is transient and can be retried.
    /// </summary>
    public bool IsTransient { get; set; }
}