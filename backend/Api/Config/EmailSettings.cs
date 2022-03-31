namespace YouFoos.Api.Config
{
    /// <summary>
    /// The configuration settings that YouFoos uses to send emails to users.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// The port that the email server is listening to.
        /// </summary>
        public int PrimaryPort { get; set; }

        /// <summary>
        /// The domain that the SMTP server is hosted on.
        /// </summary>
        public string PrimaryDomain { get; set; }

        /// <summary>
        /// The name of the person sending the email.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// The email address of the sender.
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// The username of the sender's email account.
        /// </summary>
        public string AccountUsername { get; set; }

        /// <summary>
        /// The password of the sender's email account.
        /// </summary>
        public string AccountPassword { get; set; }
    }
}
