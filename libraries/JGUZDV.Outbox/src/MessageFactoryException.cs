namespace JGUZDV.Outbox;

/// <summary>
/// Represents an exception that occurs when creating a message fails.
/// </summary>
public class MessageFactoryException : OutboxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFactoryException"/> class with a specified error message.
    /// </summary>
    public MessageFactoryException(string message) 
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFactoryException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    public MessageFactoryException(string message, Exception innerException) 
        : base(message, innerException)
    { }

    /// <summary>
    /// Gets or sets a value indicating whether the exception is transient and can be retried.
    /// </summary>
    public bool IsTransient { get; set; }
}
