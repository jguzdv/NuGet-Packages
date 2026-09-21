using System;
using System.Collections.Generic;
using System.Text;

using JGUZDV.Outbox.Abstractions;

using MimeKit;

namespace JGUZDV.Outbox.Email.Mailkit;

public class MimeMessageFactory : IMessageFactory<MimeMessage>
{
    public Task<bool> CanCreateMessage(OutboxMessage message)
        => Task.FromResult(Constants.MessageType.Equals(message.MessageType));

    public Task<MessageFactoryResult<MimeMessage>> CreateMessage(OutboxMessage message)
    {
        throw new NotImplementedException();
    }
}
