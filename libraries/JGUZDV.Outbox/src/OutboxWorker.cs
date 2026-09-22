using JGUZDV.Outbox.Abstractions;

using Microsoft.Extensions.Logging;

namespace JGUZDV.Outbox;

/// <summary>
/// Represents a worker that processes unsent messages from the outbox, attempting to send them using the appropriate message sender.
/// The worker retrieves unsent messages from the message storage, selects the appropriate sender for each message, and attempts to send the message.
/// If sending fails, the message is marked as failed in the storage.
/// </summary>
public class OutboxWorker
{
    private readonly IOutboxMessageStorage _messageStorage;
    private readonly IEnumerable<IOutboxMessageSender> _messageSenders;
    private readonly ILogger<OutboxWorker> _logger;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the OutboxWorker class with the specified message storage, message senders, time provider, and logger.
    /// </summary>
    public OutboxWorker(
        IOutboxMessageStorage messageStorage,
        IEnumerable<IOutboxMessageSender> messageSenders,
        TimeProvider timeProvider,
        ILogger<OutboxWorker> logger
        )
    {
        if (!messageSenders.Any())
        {
            throw new InvalidOperationException($"No {nameof(IOutboxMessageSender)} registered.");
        }

        _messageStorage = messageStorage;
        _messageSenders = messageSenders;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <summary>
    /// Executes the outbox worker, processing unsent messages and attempting to send them using the appropriate message sender.
    /// </summary>
    public async Task<OutboxWorkerResult> Execute(CancellationToken ct = default)
    {
        var result = new OutboxWorkerResult();

        _logger.LogDebug($"{nameof(OutboxWorker)} started.");

        var dueBefore = _timeProvider.GetUtcNow();
        var unsentMessages = await _messageStorage.GetUnsentMessages(dueBefore, 100, ct);

        foreach(var message in unsentMessages)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                var sender = await SelectMessageSender(message, ct);
                if (sender == null)
                {
                    _logger.LogError($"No sender found for message with id: {message.Id} and type: {message.MessageType}");
                    continue;
                }

                await sender.SendMessageAsync(message, ct);
                await _messageStorage.MarkAsSentAsync(message, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while processing message with id: {message.Id}");
                //TODO: Message discarding logic, e.g. if message.FailCount >= 5 or permanent error
                await _messageStorage.MarkAsFailedAsync(message, e, discard: false, ct);
                result.Exceptions.Add(e);

            }
        }

        return result;
    }

    private async Task<IOutboxMessageSender?> SelectMessageSender(OutboxMessage message, CancellationToken ct)
    {
        foreach (var sender in _messageSenders)
        {
            if (await sender.CanExecuteAsync(message, ct) && await sender.ShouldExecuteAsync(message, ct))
            {
                return sender;
            }
        }

        return null;
    }
}

/// <summary>
/// Represents the result of executing the OutboxWorker, including any exceptions that occurred during processing.
/// </summary>
public class OutboxWorkerResult
{
    /// <summary>
    /// Gets a value indicating whether the execution was successful (i.e., no exceptions occurred).
    /// </summary>
    public bool IsSuccessful => !Exceptions.Any();

    /// <summary>
    /// Gets the list of exceptions that occurred during the execution of the OutboxWorker.
    /// </summary>
    public List<Exception> Exceptions { get; } = [];
}