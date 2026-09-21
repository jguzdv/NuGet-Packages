namespace JGUZDV.Outbox.Email;

/// <summary>
/// Provides extension methods for creating OutboxMessage instances representing email messages, including plain text and HTML emails.
/// </summary>
public static class OutboxEmailMessageExtensions
{
    extension(OutboxMessage message)
    {
        /// <summary>
        /// Creates a new OutboxMessage instance representing a plain text email message with the specified due date, subject, body, and recipients.
        /// </summary>
        public static OutboxMessage CreatePlainTextEmail(
            DateTimeOffset dueDate,

            string subject,
            string body,

            List<string> to,
            List<string>? cc = null,
            List<string>? bcc = null
        ) => CreateEmail(dueDate, subject, body, false, to, cc, bcc);

        /// <summary>
        /// Creates a new OutboxMessage instance representing an HTML email message with the specified due date, subject, body, and recipients.
        /// </summary>
        public static OutboxMessage CreateHtmlEmail(
            DateTimeOffset dueDate,
            string subject,
            string htmlBody,
            List<string> to,
            List<string>? cc = null,
            List<string>? bcc = null
        ) => CreateEmail(dueDate, subject, htmlBody, true, to, cc, bcc);


        /// <summary>
        /// Creates a new OutboxMessage instance representing an email message with the specified due date, subject, body, and recipients.
        /// </summary>
        public static OutboxMessage CreateEmail(
            DateTimeOffset dueDate,
            string subject,
            string body,
            bool hasHtmlBody,
            List<string> to,
            List<string>? cc = null,
            List<string>? bcc = null
        ) => OutboxMessage.Create(
            dueDate, 
            Constants.MessageType, 
            new EmailMessageData()
            {
                Subject = subject,
                Body = body,
                IsHtmlBody = hasHtmlBody,

                EmailRecipients = new()
                {
                    To = to,
                    Cc = cc,
                    Bcc = bcc
                }
            });


        /// <summary>
        /// Tries to retrieve the EmailMessageData from the OutboxMessage if the message type is "Email".
        /// </summary>
        /// <exception cref="InvalidOperationException">If the message type is not "Email".</exception>
        public bool TryGetEmailMessageData(out EmailMessageData? messageData)
        {
            return message.MessageType == "Email"
                ? message.TryGetJsonMessageData<EmailMessageData>(out messageData)
                : throw new InvalidOperationException($"The message type '{message.MessageType}' is not supported for email messages.");
        }
    }
}
