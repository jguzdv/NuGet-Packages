namespace JGUZDV.Outbox.Email
{
    /// <summary>
    /// Represents an email message that is stored in the outbox for later sending.
    /// </summary>
    public class EmailMessageData
    {
        /// <summary>
        /// Gets or sets the recipients of the email message.
        /// </summary>
        public required EmailRecipients EmailRecipients { get; set; }

        /// <summary>
        /// Gets or sets the subject of the email message.
        /// </summary>
        public required string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body content of the email message.
        /// </summary>
        public required string Body { get; set; }

        /// <summary>
        /// Gets or sets the content type of the body content. E.g. , "text/plain" or "text/html".
        /// </summary>
        public string BodyContentType { get; set; } = "text/plain";
    }
}
