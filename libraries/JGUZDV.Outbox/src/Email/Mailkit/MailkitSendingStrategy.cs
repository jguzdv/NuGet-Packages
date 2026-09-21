using JGUZDV.Outbox.Abstractions;

using MimeKit;

namespace JGUZDV.Outbox.Email.Mailkit;

/// <summary>
/// Represents a message sender that uses Mailkit to send email messages.
/// </summary>
public class MailkitMessageSender : IOutboxMessageSender
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailkitMessageSender"/> class.
    /// </summary>
    public MailkitMessageSender(IEnumerable<IMessageFactory<MimeMessage>> messageFactories)
    {
        _messageFactories = messageFactories;
    }

    private readonly IEnumerable<IMessageFactory<MimeMessage>> _messageFactories;

    /// <summary>
    /// Checks if the type of the message is 'Email'
    /// </summary>
    public async Task<bool> CanExecute(OutboxMessage message)
    {
        foreach (var factory in _messageFactories)
        {
            if (await factory.CanCreateMessage(message))
            {
                return true;
            }
        }

        return false;
    }
        

    /// <summary>
    /// Returns true.
    /// </summary>
    public virtual Task<bool> ShouldExecute(OutboxMessage message)
        => Task.FromResult(true);

    /// <inheritdoc />
    public async Task<MessageSenderResult> SendMessage(OutboxMessage message)
    {
        IMessageFactory<MimeMessage>? effectiveFactory = null;
        foreach (var factory in _messageFactories)
        {
            if (await factory.CanCreateMessage(message))
            {
                effectiveFactory = factory;
                break;
            }
        }

        if (effectiveFactory == null)
        {
            throw new InvalidOperationException($"Could not find any mail generator that would create a message for type {message.MessageType}");
        }

        var mailContent = await effectiveFactory.CreateMessage(message);
        if (mailContent.State != MessageFactoryState.Success)
        {
            return new MessageSenderResult
            {
                MessageGenerationState = mailContent.State,
                MessageGenerationException = mailContent.Exception
            };
        }

        Exception? sendException = null;
        for (var attempt = 1; attempt <= MaxSendAttempts; attempt++)
        {
            try
            {
                await EnsureConnected();
                await _smtpClient.SendAsync(mail.Message);
                sendException = null;
                break;
            }
            catch (Exception ex)
            {
                sendException = ex;

                try
                {
                    if (_smtpClient.IsConnected)
                    {
                        await _smtpClient.DisconnectAsync(true);
                    }
                }
                catch
                {
                }

                if (attempt < MaxSendAttempts)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(RetryDelay.TotalMilliseconds * attempt));
                }
            }
        }

        if (sendException != null)
        {
            return new MessagingStrategyResult
            {
                MessageGenerationState = mail.State,
                MessageGenerationException = mail.Exception,
                Exception = sendException
            };
        }

        return new MessagingStrategyResult
        {
            MessageGenerationState = mail.State,
            MessageGenerationException = mail.Exception
        };
    }

    private Task EnsureConnected()
    {
        if (_smtpClient.IsConnected) { return Task.CompletedTask; }

        if (_options.Value.SocketOptions.HasValue)
        {
            return _smtpClient.ConnectAsync(_options.Value.Host, _options.Value.Port, _options.Value.SocketOptions.Value);
        }
        else
        {
            return _smtpClient.ConnectAsync(_options.Value.Host, _options.Value.Port, _options.Value.UseSSL!.Value);
        }
    }
}

public class MailkitOutboxSenderOptions
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
}