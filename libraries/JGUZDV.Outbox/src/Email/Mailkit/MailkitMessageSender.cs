using JGUZDV.Outbox.Abstractions;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

namespace JGUZDV.Outbox.Email.Mailkit;

/// <summary>
/// Represents a message sender that uses Mailkit to send email messages.
/// </summary>
public abstract class MailkitMessageSender<TMessageData> : IOutboxMessageSender
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailkitMessageSender{TMessageData}"/> class.
    /// </summary>
    public MailkitMessageSender(
        IEnumerable<IMessageFactory<TMessageData>> messageFactories,
        SmtpClient smtpClient,
        IOptions<MailkitMessageSenderOptions> options
    )
    {
        if (!messageFactories.Any())
        {
            throw new InvalidOperationException("No message factories were provided. At least one message factory is required.");
        }

        _messageFactories = messageFactories;
        _smtpClient = smtpClient;

        Options = options;
    }

    private readonly IEnumerable<IMessageFactory<TMessageData>> _messageFactories;
    private readonly SmtpClient _smtpClient;

    /// <summary>
    /// Gets the options for the Mailkit message sender.
    /// </summary>
    protected IOptions<MailkitMessageSenderOptions> Options { get; }

    /// <summary>
    /// Creates a <see cref="MimeMessage"/> from the provided message data.
    /// </summary>
    protected abstract MimeMessage CreateMimeMessageFromMessageData(TMessageData messageData);


    /// <summary>
    /// Checks if the type of the message is 'Email'
    /// </summary>
    public async Task<bool> CanExecuteAsync(OutboxMessage message, CancellationToken ct)
    {
        foreach (var factory in _messageFactories)
        {
            if (await factory.CanCreateMessageAsync(message, ct))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns true.
    /// </summary>
    public virtual Task<bool> ShouldExecuteAsync(OutboxMessage message, CancellationToken ct)
        => Task.FromResult(true);


    /// <inheritdoc />
    public async Task SendMessageAsync(OutboxMessage message, CancellationToken ct)
    {
        IMessageFactory<TMessageData>? effectiveFactory = null;
        foreach (var factory in _messageFactories)
        {
            if (await factory.CanCreateMessageAsync(message, ct))
            {
                effectiveFactory = factory;
                break;
            }
        }

        if (effectiveFactory == null)
        {
            // We regard this as transient, since the developer will likely add a factory for the message type in the future, and we don't want to block the message from being sent forever.
            throw new MessageSenderException($"Could not find any mail generator that would create a message for type {message.MessageType}") { IsTransient = true };
        }

        var mailContent = await effectiveFactory.CreateMessageAsync(message, ct);
        var mimeMessage = CreateMimeMessageFromMessageData(mailContent);


        for (var attempt = 1; attempt <= Options.Value.MaxSendAttempts; attempt++)
        {
            try
            {
                await EnsureConnected();
                await _smtpClient.SendAsync(mimeMessage);
                break;
            }
            catch (Exception ex)
            {
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

                if (attempt == Options.Value.MaxSendAttempts)
                {
                    throw new MessageSenderException($"Failed to send message after {Options.Value.MaxSendAttempts} attempts.", ex) { IsTransient = true };
                }
            }

            await Task.Delay(TimeSpan.FromMilliseconds(Options.Value.RetryDelay));
        }
    }

    private Task EnsureConnected() 
        => _smtpClient.IsConnected
            ? Task.CompletedTask
            : _smtpClient.ConnectAsync(Options.Value.SmtpHost, Options.Value.SmtpPort, Options.Value.SocketOptions);
}
