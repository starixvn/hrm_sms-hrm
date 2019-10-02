namespace Fanex.NexHR.Services
{
    using System;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Hangfire;

    public class RequestMailService : IRequestMailService
    {
        #region constant

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

        private const string _EmailTemplatePathForLeave = "~/app_data/emails/LeaveApproval.cshtml";

        #endregion constant

        private readonly IRequestRepository requestRepository;
        private readonly IEmailService emailService;

        public RequestMailService(
            IRequestRepository requestRepository,
            IEmailService emailService)
        {
            this.requestRepository = requestRepository;
            this.emailService = emailService;
        }

        public void SendMailForProcessSuccessfully(long requestId, EmployeeSession actor, RequestCommand command)
        {
            var criteria = new GetRequestByIdCriteria
            {
                RequestId = requestId
            };

            RequestEntity request = requestRepository.GetRequestById(criteria);
            var templateEmailPathForRelated = GetEmailTemplatePath(request.RequestType.Id);

            BackgroundJob.Enqueue(() => SendMailProcess(command, request, actor, templateEmailPathForRelated));
        }

        public EmailModel GetMailToAndMailCC(long requestId, int action)
        {
            var emailModel = new EmailModel();
            var criteria = new GetRequestRelatedPeopleCriteria
            {
                RequestId = requestId,
                Action = action
            };
            RequestRelatedPeopleModel relatedPeople = requestRepository.GetRequestRelatedPeople(criteria);
            if (relatedPeople.EmailSetting.AllowSendApprover)
            {
                emailModel = GetMailApprover(relatedPeople);
            }

            if (CheckGetMailVerifier(relatedPeople, emailModel.SendTo))
            {
                emailModel = GetMailVerifier(relatedPeople, emailModel);
            }

            if (relatedPeople.EmailSetting.AllowSendHR)
            {
                RelatedPeopleAllowSendHR(relatedPeople);
                emailModel = GetMailHR(relatedPeople, emailModel);
            }

            if (relatedPeople.EmailSetting.AllowSendUser)
            {
                emailModel = GetMailOwnerRequest(relatedPeople, emailModel);
            }

            if (relatedPeople.EmailSetting.AllowSendMailGroup)
            {
                emailModel = GetMailGroup(relatedPeople, emailModel);
            }

            emailModel.IsAllowSendRequester = relatedPeople.EmailSetting.AllowSendUser;
            emailModel.Sender = ConfigSettings.EmailSetting;
            return emailModel;
        }

        private static bool CheckGetMailVerifier(RequestRelatedPeopleModel relatedPeople, string sendTo)
        {
            bool isNotExistVerifier = (relatedPeople.Verifier != null && !string.IsNullOrWhiteSpace(relatedPeople.Verifier.Email));
            bool isVerifierDifferenceApprover = (relatedPeople.Verifier != null && relatedPeople.Approver != null) &&
                !relatedPeople.Verifier.Email.Equals(relatedPeople.Approver.Email, StringComparison.OrdinalIgnoreCase);
            return relatedPeople.EmailSetting.AllowSendVerifier
                   && isNotExistVerifier
                   && (string.IsNullOrWhiteSpace(sendTo))
                   || isVerifierDifferenceApprover;
        }

        public string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            if (totalDays > 1)
            {
                return string.Format(_MailContentLeave, fromDate.ToDateMonthYearFormatedDate(), toDate.ToDateMonthYearFormatedDate(), totalDays);
            }

            if (totalDays == 1)
            {
                return string.Format(_MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
            }

            return string.Format(_MailContentLeaveHalfDay, requestedDate.ToDateMonthYearFormatedDate(), fromDate.ToShortTime(), toDate.ToShortTime());
        }

        private static void RelatedPeopleAllowSendHR(RequestRelatedPeopleModel relatedPeople)
        {
            if (relatedPeople.Verifier != null)
            {
                relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Verifier.Email).ToList();
            }

            if (relatedPeople.Approver != null)
            {
                relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Approver.Email).ToList();
            }
        }

        public void SendMailProcess(RequestCommand command, RequestEntity request, EmployeeSession actor, string templateEmailPathForRelated)
        {
            byte action = GetActionProcessRequest(request.Status.Id, command);
            string commandMessage = GetCommandMessage(command);
            EmailModel emailModel = GetMailToAndMailCC(request.Id, action);

            if (!string.IsNullOrWhiteSpace(emailModel.SendTo))
            {
                SendToRelatedPeople(commandMessage, actor, request, emailModel, templateEmailPathForRelated);
            }
        }

        public byte GetActionProcessRequest(byte requestStatusId, RequestCommand command)
        {
            byte action;
            switch (command)
            {
                case RequestCommand.Accept:
                    action = (byte)SettingsSendMail.Verify;
                    break;

                case RequestCommand.NotAccept:
                    action = (byte)SettingsSendMail.NotVerify;
                    break;

                case RequestCommand.Approve:
                    action = (byte)SettingsSendMail.Approve;
                    break;

                case RequestCommand.Reject:
                    action = (byte)SettingsSendMail.Reject;
                    break;

                case RequestCommand.Process:
                    action = (byte)SettingsSendMail.Process;
                    break;

                case RequestCommand.Void:
                    action = (byte)SettingsSendMail.Void;
                    break;

                case RequestCommand.Reset:
                    action = GetActionResetRequest(requestStatusId);
                    break;

                default:
                    action = (byte)SettingsSendMail.New;
                    break;
            }

            return action;
        }

        public string GetCommandMessage(RequestCommand command)
        {
            string message;
            switch (command)
            {
                case RequestCommand.Accept:
                    message = string.Format(Messages.HasBeen, Labels.Accepted.ToLowerInvariant());
                    break;

                case RequestCommand.NotAccept:
                    message = string.Format(Messages.HasNotBeen, Labels.Accepted.ToLowerInvariant());
                    break;

                case RequestCommand.Approve:
                    message = string.Format(Messages.HasBeen, Labels.Approved.ToLowerInvariant());
                    break;

                case RequestCommand.Reject:
                    message = string.Format(Messages.HasBeen, Labels.Rejected.ToLowerInvariant());
                    break;

                case RequestCommand.Process:
                    message = string.Format(Messages.HasBeen, Labels.Processed.ToLowerInvariant());
                    break;

                case RequestCommand.Void:
                    message = string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant());
                    break;

                case RequestCommand.Reset:
                    message = string.Format(Messages.HasBeen, Labels.Reseted.ToLowerInvariant());
                    break;

                default:
                    message = string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant());
                    break;
            }

            return message;
        }

        private static byte GetActionResetRequest(byte statusId)
        {
            byte action;
            switch (statusId)
            {
                case (byte)CurrentRequestStatus.Verified:
                case (byte)CurrentRequestStatus.NotVerified:
                    action = (byte)SettingsSendMail.Reset1;
                    break;

                case (byte)CurrentRequestStatus.Approved:
                case (byte)CurrentRequestStatus.Rejected:
                    action = (byte)SettingsSendMail.Reset2;
                    break;

                default:
                    action = (byte)SettingsSendMail.Reset3;
                    break;
            }

            return action;
        }

        private static EmailModel GetMailApprover(RequestRelatedPeopleModel relatedPeople)
        {
            var emailModel = new EmailModel();
            if (relatedPeople.Approver != null)
            {
                emailModel.SendTo = relatedPeople.Approver.Email;
                emailModel.SendToNickName = relatedPeople.Approver.NickName;
            }

            return emailModel;
        }

        private static EmailModel GetMailVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Verifier != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Verifier.Email;
                    emailModel.SendToNickName = relatedPeople.Verifier.NickName;
                }
                else
                {
                    emailModel.SendTo += ";" + relatedPeople.Verifier.Email;
                }
            }

            return emailModel;
        }

        private static EmailModel GetMailHR(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.ListHR != null && relatedPeople.ListHR.Any())
            {
                var mailHRs = string.Join(";", relatedPeople.ListHR.Select(x => x.Email ?? string.Empty));
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = mailHRs;
                    emailModel.SendToNickName = Labels.HR;
                }
                else
                {
                    emailModel.SendCC = mailHRs;
                }
            }

            return emailModel;
        }

        private static EmailModel GetMailOwnerRequest(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Requester != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Requester.Email;
                    emailModel.SendToNickName = relatedPeople.Requester.NickName;
                }
                else
                {
                    emailModel.SendCC = AddMailToList(emailModel.SendCC, relatedPeople.Requester.Email);
                }
            }

            return emailModel;
        }

        private static string AddMailToList(string mailList, string mail)
        {
            string fullMailList = string.Empty;

            if (string.IsNullOrWhiteSpace(mail))
            {
                return mailList;
            }

            if (string.IsNullOrWhiteSpace(mailList))
            {
                return mail;
            }

            fullMailList = mailList + ";" + mail;

            return fullMailList;
        }

        private static EmailModel GetMailGroup(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Requester != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Requester.MailGroup;
                    emailModel.SendToNickName = Labels.All;
                }
                else if (string.IsNullOrWhiteSpace(emailModel.SendCC))
                {
                    emailModel.SendCC = relatedPeople.Requester.MailGroup;
                }
                else
                {
                    emailModel.SendCC += ";" + relatedPeople.Requester.MailGroup;
                }
            }

            return emailModel;
        }

        private static string GetEmailTemplatePath(short requestType)
        {
            return requestType == (short)RequestType.Leave ? HttpContext.Current.Server.MapPath(_EmailTemplatePathForLeave) : string.Empty;
        }

        private void SendToRelatedPeople(
            string command,
            EmployeeSession actor,
            RequestEntity request,
            EmailModel emailModel,
            string templateEmailPath)
        {
            if (request.RequestType.Id == (short)RequestType.Leave)
            {
                emailModel.Subject = string.Format(Messages.ApprovalNotificationLeaveSubject, request.Requester.NickName, command, actor.Nickname);
                LeaveRelatedMail(command, actor, request, emailModel, templateEmailPath);
            }
        }

        private void LeaveRelatedMail(
            string command,
            EmployeeSession actor,
            RequestEntity request,
            EmailModel emailModel,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = request.TotalDays == Constants.HalfDayOffValue
                ? GetStringDateForLeaveRequest(request.FromDate, request.FromTime, request.ToTime, request.TotalDays)
                : GetStringDateForLeaveRequest(request.FromDate, fromDate: request.FromDate, toDate: request.ToDate, totalDays: request.TotalDays);
            body = string.Format(
                body,
                emailModel.SendToNickName,
                request.Requester.DisplayName,
                command,
                actor.DisplayName,
                request.RequestType.Name,
                forDate,
                request.LeaveType.Name,
                request.Remark);

            emailService.SendEmail(ConfigSettings.EmailSetting, emailModel.SendTo, emailModel.SendCC, emailModel.Subject, body);
        }
    }
}