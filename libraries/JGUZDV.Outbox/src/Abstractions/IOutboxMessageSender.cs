namespace JGUZDV.Outbox.Abstractions;

/// <summary>
/// Defines the contract for sending outbox messages. Implementations of this interface are responsible for determining if a message can be sent, whether it should be sent, and for actually sending the message.
/// This allows for different sending strategies to be implemented, such as email, SMS, or other messaging systems.
/// </summary>
public interface IOutboxMessageSender
{
    /// <summary>
    /// Determines whether the specified message can be sent by this sender. This method allows the sender to check if it is capable of handling the message based on its type, content, or other criteria.
    /// </summary>
    Task<bool> CanExecute(OutboxMessage message);

    /// <summary>
    /// Determines whether the specified message should be sent by this sender. This method allows the sender to decide if it is appropriate to send the message at this time, based on factors such as message state, timing, or other conditions.
    /// </summary>
    Task<bool> ShouldExecute(OutboxMessage message);

    /// <summary>
    /// Sends the specified outbox message. This method is responsible for the actual sending of the message and returns a result indicating the success or failure of the operation, along with any relevant exceptions or state information.
    /// </summary>
    Task<MessageSenderResult> SendMessage(OutboxMessage message);
}
