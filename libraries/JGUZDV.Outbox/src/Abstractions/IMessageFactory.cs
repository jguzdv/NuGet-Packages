namespace JGUZDV.Outbox.Abstractions;

public interface IMessageFactory<T>
{
    Task<bool> CanCreateMessage(OutboxMessage message);

    Task<MessageFactoryResult<T>> CreateMessage(OutboxMessage message);
}
