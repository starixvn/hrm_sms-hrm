namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Hangfire;

    public class LateInEarlyOutRequestService : ILateInEarlyOutRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/LateInEarlyOut.Request.cshtml";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/LateInEarlyOut.Cancel.cshtml";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/LateInEarlyOut.Approval.cshtml";
        private readonly ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly IBaseService baseService;

        public LateInEarlyOutRequestService(
            ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository,
            IRequestMailService requestMailService,
            IEmailService emailService,
            IBaseService baseService)
        {
            this.lateInEarlyOutRequestRepository = lateInEarlyOutRequestRepository;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
            this.baseService = baseService;
        }

        public IList<LateInEarlyOutRequest> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria)
        {
            IList<LateInEarlyOutRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = lateInEarlyOutRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<LateInEarlyOutRequest>();
        }

        public LateInEarlyOutRequest GetById(long requestId)
        {
            var criteria = new GetLateInEarlyOutRequestByIdCriteria
            {
                RequestId = requestId
            };
            return lateInEarlyOutRequestRepository.GetById(criteria);
        }

        public IList<LateInEarlyOutRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetLateInEarlyOutRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return lateInEarlyOutRequestRepository.GetHistoryById(criteria);
        }

        public EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria)
        {
            return baseService.GetDataEntity<EmployeeLateInEarlyOutModel>(criteria);
        }

        public IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param)
        {
            var criteria = new GetDisabledDateCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                RequestId = param.RequestId
            };
            return baseService.GetData<DateTime>(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteLateInEarlyOutRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailCreateRequest(long requestId)
        {
            LateInEarlyOutRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                EmailModel email = requestMailService.GetMailToAndMailCC(requestId, (byte)SettingsSendMail.New);
                email.Subject = string.Format(
                    "{0} {1} on {2}",
                    Messages.LateInEarlyOutEmailSubject,
                    request.Employee.NickName,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.NickName,
                    request.RequestTypeId == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins,
                    request.Remark,
                    ConfigSettings.ApplicationDomain);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));
            }
        }

        public void SendMailEditRequest(long requestId)
        {
            LateInEarlyOutRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                EmailModel email = requestMailService.GetMailToAndMailCC(requestId, (byte)SettingsSendMail.Edit);
                email.Subject = string.Format(
                    "{0} {1} on {2}",
                    Messages.UpdatedLateInEarlyOutEmailSubject,
                    request.Employee.NickName,
                    request.OnDate.ToDateMonthYearFormatedDate());

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.NickName,
                    request.RequestTypeId == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins,
                    request.Remark,
                    ConfigSettings.ApplicationDomain);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));
            }
        }

        public void SendMailDeleteRequest(long requestId)
        {
            LateInEarlyOutRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                EmailModel email = requestMailService.GetMailToAndMailCC(requestId, (byte)SettingsSendMail.Cancel);
                email.Subject = string.Format(Messages.MessageLateInEarlyOutRequest, request.RequestTypeName, request.Employee.NickName);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.DisplayName,
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
                    request.Remark);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));
            }
        }

        public void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
        {
            LateInEarlyOutRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                byte action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                string commandMessage = requestMailService.GetCommandMessage(command);
                EmailModel email = requestMailService.GetMailToAndMailCC(requestId, action);

                email.Subject = request.RequestTypeId == (byte)RequestType.LateIn
                    ? string.Format(Messages.ApprovalNotificationLateInSubject, request.Employee.NickName, commandMessage, actor.Nickname)
                    : string.Format(Messages.ApprovalNotificationEarlyOutSubject, request.Employee.NickName, commandMessage, actor.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.DisplayName,
                    commandMessage,
                    actor.DisplayName,
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (byte)RequestType.LateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
                    request.Remark);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));
            }
        }
    }
}