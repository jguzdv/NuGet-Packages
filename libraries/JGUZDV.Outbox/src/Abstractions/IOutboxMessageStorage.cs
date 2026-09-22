namespace JGUZDV.Outbox.Abstractions;

/// <summary>
/// Represents a storage mechanism for outbox messages, providing methods for recording and retrieving messages as well as marking them as sent.
/// </summary>
public interface IOutboxMessageStorage : IOutboxMessageRecorder
{
    /// <summary>
    /// Retrieves a collection of unsent outbox messages that are due before the specified date and time, limited to the specified batch size.
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetUnsentMessages(DateTimeOffset dueBefore, int batchSize, CancellationToken ct);

    /// <summary>
    /// Marks the specified outbox message as sent, indicating that it has been successfully processed and should not be retried.
    /// </summary>
    Task MarkAsSentAsync(OutboxMessage message, CancellationToken ct);

    /// <summary>
    /// Marks the specified outbox message as failed, recording the provided exception and indicating whether the message should be discarded or retried.
    /// </summary>
    Task MarkAsFailedAsync(OutboxMessage message, Exception exception, bool discard, CancellationToken ct);
}
