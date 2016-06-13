namespace Fanex.NexHR.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string senderEmail, string receiverEmail, string mailCc, string subject, string body);

        void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCc, string subject, string body);
    }
}
