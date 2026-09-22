using JGUZDV.Outbox;
using JGUZDV.Outbox.Email;
using JGUZDV.Outbox.Email.Mailkit;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring Outbox with Mailkit
/// </summary>
public static class OutboxSenderBuilderExtensions
{
    extension(OutboxSenderBuilder builder)
    {
        /// <summary>
        /// Configures the outbox sender to use Mailkit for sending emails.
        /// </summary>
        public OutboxSenderBuilder AddDefaultEmailSender(
            Action<MailkitMessageSenderOptions> configureOptions
        )
        {
            builder.AddMessageSender<MailkitEmailMessageSender, MailkitMessageSenderOptions>(configureOptions);
            builder.AddMessageFactory<EmailMessageDataFactory, EmailMessageData>();

            return builder;
        }
    }
}

