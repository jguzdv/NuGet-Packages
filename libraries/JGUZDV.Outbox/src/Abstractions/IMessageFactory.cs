namespace JGUZDV.Outbox.Abstractions;

/// <summary>
/// Defines a factory interface for creating messages of type T from an OutboxMessage.
/// </summary>
public interface IMessageFactory<T>
{
    /// <summary>
    /// Determines whether a message can be created from the given OutboxMessage.
    /// </summary>
    Task<bool> CanCreateMessage(OutboxMessage message);

    /// <summary>
    /// Creates a message of type T from the given OutboxMessage.
    /// </summary>
    Task<T> CreateMessage(OutboxMessage message);
}
