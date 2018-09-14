namespace Fanex.NexHR.Services
{
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Hangfire;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public class MessageService : BaseService, IMessageService
    {
        private readonly string messageEmailTemplate = "~/app_data/emails/MessageNotification.cshtml";

        private readonly IEmailService emailService;

        public MessageService()
            : this(null)
        {
        }

        public MessageService(IEmailService emailService)
        {
            this.emailService = emailService ?? new EmailService();
        }

        public IList<Message> GetMessageByCreator(GetMessageByCreatorCriteria criteria)
        {
            IList<Message> messages = null;
            if (criteria != null && criteria.IsValid())
            {
                messages = GetData<Message>(criteria).ToList();
            }

            return messages ?? new List<Message>();
        }

        public Message GetMessageById(GetMessageByIdCriteria criteria)
        {
            var message = new Message();
            if (criteria != null && criteria.IsValid())
            {
                message = GetDataEntity<Message>(criteria);
            }

            return message;
        }

        public IList<Message> GetMessageByEmployee(GetMessageByEmployeeCriteria criteria)
        {
            IList<Message> messages = null;
            if (criteria != null && criteria.IsValid())
            {
                messages = GetData<Message>(criteria).ToList();
            }

            return messages ?? new List<Message>();
        }

        public Message GetDetailMessageByEmployee(GetMessageOfEmployeeByIdCriteria criteria)
        {
            var message = new Message();
            if (criteria != null && criteria.IsValid())
            {
                message = GetDataEntity<Message>(criteria);
            }

            return message;
        }

        public IList<Message> GetUnreadMessageOfEmployee(GetUnreadMessageOfEmployeeCriteria criteria)
        {
            IList<Message> messages = null;
            if (criteria != null && criteria.IsValid())
            {
                messages = GetData<Message>(criteria).ToList();
            }

            return messages ?? new List<Message>();
        }

        public void SendMailToStaff(Message model)
        {
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath(messageEmailTemplate));
            BackgroundJob.Enqueue(() => SendEmail(model.Subject, model.Contents, model.SendToEmail, bodyTemplate));
        }

        public void SendEmail(string subject, string content, string sendToEmail, string emailTemplatePath)
        {
            var body = string.Format(emailTemplatePath, subject, content);

            emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                sendToEmail,
                string.Empty,
                Resources.Messages.MessageSubject + " " + subject,
                body);
        }
    }
}