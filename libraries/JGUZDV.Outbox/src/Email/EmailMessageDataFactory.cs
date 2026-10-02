using JGUZDV.Outbox.Abstractions;

namespace JGUZDV.Outbox.Email;

/// <summary>
/// Represents a factory for creating <see cref="EmailMessageData"/> instances from <see cref="OutboxMessage"/> instances.
/// </summary>
public class EmailMessageDataFactory : IMessageFactory<EmailMessageData>
{
    /// <inheritdoc />
    public Task<bool> CanCreateMessageAsync(OutboxMessage message, CancellationToken ct)
        => Task.FromResult(Constants.MessageType.Equals(message.MessageType));

    /// <inheritdoc />
    public Task<EmailMessageData> CreateMessageAsync(OutboxMessage message, CancellationToken ct)
    {
        return message.TryGetEmailMessageData(out var emailMessageData) && emailMessageData is not null
            ? Task.FromResult(emailMessageData)
            : throw new MessageFactoryException($"MessageData from message {message.Id} is not a valid {nameof(EmailMessageData)} or was empty.");
    }
}
