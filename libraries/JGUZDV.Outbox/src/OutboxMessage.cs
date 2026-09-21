using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

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
    /// Gets or sets the due date for processing or sending the message.
    /// </summary>
    public DateTimeOffset DueDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the message has been sent. If the message has not been sent yet, this property will be null.
    /// </summary>
    public DateTimeOffset? HasBeenSent { get; set; }


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
    /// Gets or sets a list of failure information related to the message processing. If there are no failures, this property will be null.
    /// </summary>
    public List<FailureInfo>? Failures { get; set; }

    /// <summary>
    /// Gets the count of failures that have occurred during message processing. This property is read-only and is updated internally when failures are recorded.
    /// </summary>
    public int FailCount { get; internal set; }



    /// <summary>
    /// Creates a new instance of the OutboxMessage class with the specified due date, message type, and message data, serialized as JSON.
    /// </summary>
    public static OutboxMessage CreateWithJson<TMessageData>(DateTimeOffset dueDate, string messageType, TMessageData messageData)
    {
        var jsonMessageData = JsonSerializer.Serialize(messageData);
        return new OutboxMessage(dueDate, messageType, jsonMessageData);
    }

    /// <summary>
    /// Gets the message data deserialized from JSON to the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool TryGetJsonMessageData<T>(out T messageData)
    {
        /// <summary>
        /// Gets the message data deserialized from JSON to the specified type.
        /// </summary>
        try
        {
            messageData = JsonSerializer.Deserialize<T>(MessageData)!;
            return true;
        }
        catch
        {
            messageData = default!;
            return false;
        }
    }
}
