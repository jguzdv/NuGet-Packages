namespace JGUZDV.Outbox.Abstractions;

/// <summary>
/// Defines an interface for recording messages into an outbox for later processing, allowing for asynchronous message enqueueing.
/// The implementer might OR might not implement unit-of-work pattern.
/// </summary>
public interface IOutboxMessageRecorder
{
    /// <summary>
    /// Puts a message into the outbox for later processing.
    /// </summary>
    Task EnqueueMessage(OutboxMessage message);

    /// <summary>
    /// Saves all changes made in this context to the underlying database asynchronously.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct);
}
