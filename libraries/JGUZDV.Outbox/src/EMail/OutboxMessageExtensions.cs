namespace JGUZDV.Outbox.EMail;

public static class OutboxMessageExtensions
{
    extension(OutboxMessage)
    {
        public static OutboxMessage CreatePlainTextEmail(
            DateTimeOffset dueDate,

            string subject,
            string body,

            List<string> to,
            List<string>? cc = null,
            List<string>? bcc = null
        ) => CreateEmail(dueDate, subject, body, false, to, cc, bcc);

        public static OutboxMessage CreateHtmlEmail(
            DateTimeOffset dueDate,
            string subject,
            string htmlBody,
            List<string> to,
            List<string>? cc = null,
            List<string>? bcc = null
        ) => CreateEmail(dueDate, subject, htmlBody, true, to, cc, bcc);


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
            "Email", 
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
    }
}
