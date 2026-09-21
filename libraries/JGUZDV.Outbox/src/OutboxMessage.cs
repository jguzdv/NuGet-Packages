using System.Diagnostics.CodeAnalysis;

namespace JGUZDV.Outbox;

/// <summary>
/// Represents a message that is stored in the outbox for later processing or sending.
/// This class is abstract and serves as a base for specific message types.
/// </summary>
public class OutboxMessage
{
    internal OutboxMessage() { }

    /// <summary>
    /// Creates a new instance of the OutboxMessage class with the specified due date, message type, and message data.
    /// </summary>
    [SetsRequiredMembers]
    public OutboxMessage(DateTimeOffset dueDate, string messageType, string messageData)
    {
        DueDate = dueDate;

        MessageType = messageType;
        MessageData = messageData;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the outbox message.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets a value indicating, when the message has been addded to the outbox. 
    /// This property is set automatically when the message is recorded in the outbox and should not be modified manually.
    /// </summary>
    public DateTimeOffset RecordDate { get; set; }

    /// <summary>
    /// Gets or sets the due date for processing or sending the message.
    /// </summary>
    public DateTimeOffset DueDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the message has been sent. If the message has not been sent yet, this property will be null.
    /// </summary>
    public DateTimeOffset? SentAtDate { get; set; }


    /// <summary>
    /// Gets or sets the type of the message.
    /// This type is meant as discriminator for the message data and can be used to determine how to process or handle the message.
    /// </summary>
    public required string MessageType { get; set; }

    /// <summary>
    /// Gets or sets the data of the message.
    /// Anything that is stored in this property should be serializable and deserializable to and from the specified message type.
    /// </summary>
    public required string MessageData { get; set; }

    /// <summary>
    /// Gets or sets an optional string of tags associated with the message.
    /// This can be used to find messages again after putting them into the outbox, e.g. to delete them, when it's not relevant anymore.
    /// </summary>
    public string? Tags { get; set; }


    /// <summary>
    /// Gets or sets the current state of the message, which can be used to track its processing status.
    /// </summary>
    public string? MessageState { get; set; }

    /// <summary>
    /// Gets or sets a list of failure information related to the message processing. If there are no failures, this property will be null.
    /// </summary>
    public List<FailureInfo>? Failures { get; set; }

    /// <summary>
    /// Gets the count of failures that have occurred during message processing. This property is read-only and is updated internally when failures are recorded.
    /// </summary>
    public int FailCount { get; internal set; }
}
