namespace Fanex.NexHR.MailDeliver
{
    using Core.Services;
    using Criteria;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using System;
    using System.Configuration;
    using System.Text.RegularExpressions;

    public class BaseMailService
    {
        private readonly BaseService baseService;
        private readonly ICommandService commandService;

        public BaseMailService() : this(null, null) { }

        public BaseMailService(BaseService baseService, ICommandService commandService)
        {
            this.baseService = baseService ?? new BaseService();
            this.commandService = commandService ?? new CommandService();
        }

        public bool SendEmailAsync(MailRecord email)
        {
            var mailStatus = new MailStatus();
            if (email != null && IsValidEmail(email.MailTo))
            {
                email.MailSetting = GetMailSettingById(int.Parse(ConfigurationManager.AppSettings["app_mail_id"]));
                InsertMailRecord(email);
                var message = GetMessageContent(email);
                mailStatus = Send(email, message);
            }

            return mailStatus.IsSucessfull;
        }

        private MailStatus Send(MailRecord email, MimeMessage message)
        {
            var mailStatus = new MailStatus();

            using (var mailclient = new SmtpClient())
            {
                mailclient.Connect(email.MailSetting.Server, email.MailSetting.Port, email.MailSetting.IsAuth);
                if (email.MailSetting.IsAuth)
                {
                    mailclient.AuthenticateAsync(email.MailSetting.Username, email.MailSetting.Password);
                }

                try
                {
                    mailclient.SendAsync(message);
                    UpdateMailStatus(email, true);
                }
                catch (AuthenticationException ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw ex;
                }
                catch (SmtpCommandException ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw ex;
                }
                catch (SmtpProtocolException ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw ex;
                }
                catch (Exception ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw ex;
                }

                mailclient.Disconnect(true);
            }

            return mailStatus;
        }

        private MimeMessage GetMessageContent(MailRecord email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(email.MailSetting.Username));
            message.To.Add(new MailboxAddress(email.MailTo));
            message.Subject = email.Subject;
            message.Priority = (MessagePriority)email.Priority;

            var builder = new BodyBuilder
            {
                HtmlBody = email.Body
            };
            foreach (MimeEntity attachment in email.Attachments)
            {
                builder.Attachments.Add(attachment);
            }

            message.Body = builder.ToMessageBody();

            return message;
        }

        private MailSetting GetMailSettingById(int appId)
        {
            var criteria = new GetMailSettingByIdCriteria
            {
                SettingId = appId
            };

            var mailSetting = baseService.GetDataEntity<MailSetting>(criteria);
            return mailSetting;
        }

        private void InsertMailRecord(MailRecord email)
        {
            var criteria = new InsertMailRecordCriteria
            {
                AppId = email.MailSetting.AppId,
                Subject = email.Subject,
                Body = email.Body,
                Attachments = string.Empty,
                MailTo = email.MailTo,
                MailCc = email.MailCc,
                MailBcc = email.MailBcc,
                MailDate = DateTime.Now
            };

            commandService.Insert(criteria, hasGetOutputParams: true);
            email.MailId = criteria.MailId;
        }

        private void HandleMailError(MailRecord email, string errorMessage, string stackTrace)
        {
            InsertMailError(email, errorMessage, stackTrace);
            UpdateMailStatus(email, false);
        }

        private void InsertMailError(MailRecord email, string errorMessage, string stackTrace)
        {
            var criteria = new InsertMailErrorCriteria
            {
                AppId = email.MailSetting.AppId,
                MailId = email.MailId,
                Message = errorMessage,
                StackTrace = stackTrace,
                ErrorTime = DateTime.Now
            };

            commandService.Insert(criteria);
        }

        private void UpdateMailStatus(MailRecord email, bool isSucessfull)
        {
            var criteria = new UpdateMailStatusCriteria
            {
                MailId = email.MailId,
                Status = isSucessfull,
                MailUpdatedDate = DateTime.Now
            };

            commandService.Update(criteria);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }
    }
}
