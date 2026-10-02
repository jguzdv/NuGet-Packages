namespace JGUZDV.Outbox.Email
{
    /// <summary>
    /// Represents the recipients of an email message, including To, Cc, and Bcc addresses.
    /// </summary>
    public class EmailRecipients
    {
        /// <summary>
        /// The list of email addresses to which the email will be sent. 
        /// This property is required and must contain at least one valid email address.
        /// </summary>
        public required List<string> To { get; set; }

        /// <summary>
        /// The list of email addresses to which the email will be sent as carbon copy (CC). 
        /// This property is optional and can be null or empty if no CC recipients are specified.
        /// </summary>
        public List<string>? Cc { get; set; }

        /// <summary>
        /// The list of email addresses to which the email will be sent as blind carbon copy (BCC). 
        /// This property is optional and can be null or empty if no BCC recipients are specified.
        /// </summary>
        public List<string>? Bcc { get; set; }
    }
}
