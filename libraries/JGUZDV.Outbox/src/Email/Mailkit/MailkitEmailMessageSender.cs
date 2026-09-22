using JGUZDV.Outbox.Abstractions;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

namespace JGUZDV.Outbox.Email.Mailkit;

/// <summary>
/// Represents a message sender that uses Mailkit to send email messages.
/// </summary>
public class MailkitEmailMessageSender : MailkitMessageSender<EmailMessageData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MailkitEmailMessageSender"/> class.
    /// </summary>
    public MailkitEmailMessageSender(
        IEnumerable<IMessageFactory<EmailMessageData>> messageFactories, 
        SmtpClient smtpClient, 
        IOptions<MailkitMessageSenderOptions> options) 
        : base(messageFactories, smtpClient, options)
    { }

    /// <inheritdoc />
    protected override MimeMessage CreateMimeMessageFromMessageData(EmailMessageData messageData)
    {
        var from = Options.Value.FromAddress;

        var result = new MimeMessage()
        {
            Subject = messageData.Subject,
            Body = new TextPart(messageData.IsHtmlBody ? "html" : "plain")
            {
                Text = messageData.Body
            },
        };

        foreach (var to in messageData.EmailRecipients.To)
        {
            result.To.Add(new MailboxAddress(null, to));
        }
        foreach (var cc in messageData.EmailRecipients.Cc ?? [])
        {
            result.Cc.Add(new MailboxAddress(null, cc));
        }
        foreach (var bcc in messageData.EmailRecipients.Bcc ?? [])
        {
            result.Bcc.Add(new MailboxAddress(null, bcc));
        }

        return result;
    }
}
