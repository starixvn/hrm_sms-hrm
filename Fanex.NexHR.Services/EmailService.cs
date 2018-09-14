namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;
    using MailDeliver;

    public class EmailService : IEmailService
    {
        public void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body)
        {
            SendEmail(senderEmail, senderEmail, receiverEmail, mailCC, subject, body);
        }

        public void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCC, string subject, string body)
        {
            if (!Utilities.IsValidEmail(senderEmail) || !Utilities.IsValidEmail(receiverEmail) || ConfigSettings.IsDisabledSendMail)
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCC,
                Subject = subject,
                Body = body
            };

            var mailservice = new BaseMailService();
            mailservice.SendEmailAsync(mailItem);
        }
    }
}
