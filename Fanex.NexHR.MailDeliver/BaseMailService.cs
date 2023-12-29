namespace Fanex.NexHR.MailDeliver
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Criteria;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    public class BaseMailService : IBaseMailService
    {
        private readonly BaseService baseService;

        public BaseMailService(BaseService baseService)
        {
            this.baseService = baseService;
        }

        public bool SendEmailAsync(MailRecord email)
        {
            var isSucessfull = false;
            if (email != null && Utilities.IsValidEmail(email.MailTo))
            {
                email.MailSetting = GetMailSettingById(int.Parse(ConfigurationManager.AppSettings["app_mail_id"]));
                InsertMailRecord(email);
                isSucessfull = Send(email);
            }

            return isSucessfull;
        }

        private bool Send(MailRecord email)
        {
            var isSucessfull = email.MailSetting.Port == 465 ? SendByMailKit(email) : SendByMailNet(email);
            return isSucessfull;
        }

        private bool SendByMailKit(MailRecord email)
        {
            MimeMessage message = GetMessageContent(email);
            var isSucessfull = false;

            using (var mailclient = new MailKit.Net.Smtp.SmtpClient())
            {
                mailclient.Connect(email.MailSetting.Server, email.MailSetting.Port, email.MailSetting.IsAuth);
                if (email.MailSetting.IsAuth)
                {
                    mailclient.Authenticate(email.MailSetting.Username, email.MailSetting.Password);
                }

                try
                {
                    mailclient.Send(message);
                    isSucessfull = true;
                    UpdateMailStatus(email, isSucessfull);
                }
                catch (AuthenticationException ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw;
                }
                catch (SmtpCommandException ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw;
                }
                catch (SmtpProtocolException ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw;
                }
                catch (Exception ex)
                {
                    HandleMailError(email, ex.Message, ex.StackTrace);
                    throw;
                }

                mailclient.Disconnect(true);
            }

            return isSucessfull;
        }

        private bool SendByMailNet(MailRecord email)
        {
            var isSucessfull = false;
            try
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(email.MailSetting.Username)
                };
                if (!string.IsNullOrEmpty(email.MailTo))
                {
                    GetMailFromString(email.MailTo, msg.To);
                    GetMailFromString(email.MailCc, msg.CC);
                    GetMailFromString(email.MailBcc, msg.Bcc);

                    msg.Subject = email.Subject;
                    msg.SubjectEncoding = Encoding.UTF8;
                    msg.IsBodyHtml = true;
                    msg.Body = email.Body;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.Priority = MailPriority.Normal;

                    var client = new System.Net.Mail.SmtpClient
                    {
                        Port = email.MailSetting.Port,
                        Host = email.MailSetting.Server
                    };
                    if (email.MailSetting.IsAuth)
                    {
                        client.Credentials = new System.Net.NetworkCredential(email.MailSetting.Username, email.MailSetting.Password);
                        client.EnableSsl = true;
                    }

                    ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;

                    client.SendCompleted += SendCompletedCallback;
                    client.SendAsync(msg, email);
                    isSucessfull = true;
                }
            }
            catch (Exception ex)
            {
                HandleMailError(email, ex.Message, ex.StackTrace);
                throw;
            }

            return isSucessfull;
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            var email = (MailRecord)e.UserState;
            bool status = e.Error == null;
            if (!status)
            {
                HandleMailError(email, (e.Error.InnerException != null ? e.Error.InnerException.Message : e.Error.Message), e.Error.StackTrace);
            }
            else
            {
                UpdateMailStatus(email, true);
            }
        }

        private static void GetMailFromString(string mailString, MailAddressCollection mails)
        {
            if (!string.IsNullOrWhiteSpace(mailString))
            {
                var emails = RemoveDoubleComma(mailString);
                var mailArrs = emails.Split(';');
                foreach (var mail in mailArrs)
                {
                    if (!string.IsNullOrEmpty(mail))
                    {
                        mails.Add(mail);
                    }
                }
            }
        }

        private static void GetMailAddressFromString(string mailString, InternetAddressList mails)
        {
            if (!string.IsNullOrWhiteSpace(mailString))
            {
                var emails = RemoveDoubleComma(mailString);
                var mailArrs = emails.Split(';');
                foreach (var mail in mailArrs)
                {
                    if (!string.IsNullOrEmpty(mail))
                    {
                        mails.Add(new MailboxAddress(string.Empty, mail));
                    }
                }
            }
        }

        private static string RemoveDoubleComma(string mails)
        {
            var emails = mails;
            while (emails.Contains(",,"))
            {
                emails = emails.Replace(",,", ",");
                emails = emails.Replace(",", ";");
            }

            while (emails.Contains(";;"))
            {
                emails = emails.Replace(";;", ";");
            }

            return emails.TrimEnd(',', ';');
        }

        private MimeMessage GetMessageContent(MailRecord email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(string.Empty, email.MailSetting.Username));
            GetMailAddressFromString(email.MailTo, message.To);
            GetMailAddressFromString(email.MailCc, message.Cc);
            GetMailAddressFromString(email.MailBcc, message.Bcc);

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
                MailDate = DateTimeOffset.Now.DateTime
            };

            baseService.ExecuteNonQuery(criteria);
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
                ErrorTime = DateTimeOffset.Now.DateTime
            };

            baseService.ExecuteNonQuery(criteria);
        }

        private void UpdateMailStatus(MailRecord email, bool isSucessfull)
        {
            var criteria = new UpdateMailStatusCriteria
            {
                MailId = email.MailId,
                Status = isSucessfull,
                MailUpdatedDate = DateTimeOffset.Now.DateTime
            };

            baseService.ExecuteNonQuery(criteria);
        }

        private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;
    }
}