using MailKit.Security;

using MimeKit;

namespace JGUZDV.Outbox.Email.Mailkit;

/// <summary>
/// Represents the options for configuring the Mailkit message sender.
/// </summary>
public class MailkitMessageSenderOptions
{
    /// <summary>
    /// Gets or sets the email address that will be used as the sender of the email messages.
    /// </summary>
    public required MailboxAddress FromAddress { get; set; }

    /// <summary>
    /// Gets or sets the SMTP host that will be used to send the email messages.
    /// </summary>
    public required string SmtpHost { get; set; }

    /// <summary>
    /// Gets or sets the SMTP port that will be used to send the email messages. The default value is 587.
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Gets or sets the SMTP username that will be used to authenticate with the SMTP server.
    /// </summary>
    public string? SmtpUsername { get; set; }

    /// <summary>
    /// Gets or sets the SMTP password that will be used to authenticate with the SMTP server.
    /// </summary>
    public string? SmtpPassword { get; set; }

    /// <summary>
    /// Gets or sets the secure socket options that will be used to connect to the SMTP server. The default value is <see cref="SecureSocketOptions.Auto"/>.
    /// </summary>
    public SecureSocketOptions SocketOptions { get; set; } = SecureSocketOptions.Auto;


    /// <summary>
    /// Gets or sets the delay between retry attempts when sending email messages. The default value is 100 milliseconds.
    /// </summary>
    public int RetryDelay { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum number of send attempts when sending email messages. The default value is 3.
    /// </summary>
    public int MaxSendAttempts { get; set; } = 3;
}