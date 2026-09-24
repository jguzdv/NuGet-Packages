namespace JGUZDV.Outbox.Abstractions;

/// <summary>
/// Represents a storage mechanism for outbox messages, providing methods for recording and retrieving messages as well as marking them as sent.
/// </summary>
public interface IOutboxMessageStorage : IOutboxMessageRecorder
{
    /// <summary>
    /// Retrieves a collection of unsent outbox messages that are due before the specified date and time, limited to the specified batch size.
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetUnsentMessagesAsync(DateTimeOffset dueBefore, int batchSize, CancellationToken ct);

    /// <summary>
    /// Retrieves a collection of unsent outbox messages of the specified type that are due before the specified date and time, limited to the specified batch size.
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetMessagesAsync(MessageQuery query, CancellationToken ct);

    /// <summary>
    /// Removes the specified outbox message from the storage, indicating that it has been successfully processed and should no longer be retained.
    /// </summary>
    Task RemoveMessageAsync(OutboxMessage message, CancellationToken ct);

    /// <summary>
    /// Marks the specified outbox message as sent, indicating that it has been successfully processed and should not be retried.
    /// </summary>
    Task MarkAsSentAsync(OutboxMessage message, CancellationToken ct);

    /// <summary>
    /// Marks the specified outbox message as failed, recording the provided exception and indicating whether the message should be discarded or retried.
    /// </summary>
    Task MarkAsFailedAsync(OutboxMessage message, Exception exception, bool discard, CancellationToken ct);
}

/// <summary>
/// Represents a query for retrieving outbox messages based on various criteria, including due date, message type, tags, and whether to include sent messages.
/// </summary>
/// <param name="DueBefore">The date and time before which messages are due.</param>
/// <param name="DueAfter">The date and time after which messages are due.</param>
/// <param name="MessageType">The type of messages to retrieve.</param>
/// <param name="Tags">The tags associated with messages to retrieve.</param>
/// <param name="IncludeProcessedMessages">Whether to include messages that have already been sent.</param>
public record MessageQuery(
    DateTimeOffset? DueBefore = null,
    DateTimeOffset? DueAfter = null,
    string? MessageType = null,
    string? Tags = null,
    bool IncludeProcessedMessages = false);