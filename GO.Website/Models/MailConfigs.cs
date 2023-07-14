namespace GO.Website.Models
{
    using MimeKit;

    public class MailConfigs
    {
        public bool Enabled { get; set; }
        public string Domain { get; set; }
        public string SenderName { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public MessagePriority MailPriority { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}