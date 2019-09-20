namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Hangfire;

    public class ChangeShiftRequestService : IChangeShiftRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/ChangeShift.Request.cshtml";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/ChangeShift.Cancel.cshtml";

        private readonly IChangeShiftRequestRepository changeShiftRepository;
        private readonly IBaseService baseService;
        private readonly IEmailService emailService;
        private readonly IRequestMailService requestMailService;

        public ChangeShiftRequestService(
            IChangeShiftRequestRepository changeShiftRepository,
            IBaseService baseService,
            IEmailService emailService,
            IRequestMailService requestMailService)
        {
            this.changeShiftRepository = changeShiftRepository;
            this.baseService = baseService;
            this.emailService = emailService;
            this.requestMailService = requestMailService;
        }

        public int GetNumberOfRequest(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetNumberOfRequestCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return baseService.GetDataEntity<int>(criteria);
        }

        public IEnumerable<DailySchedule> GetWorkingCalendarForRequest(GetWorkingCalendarForRequestParam param)
        {
            var criteria = new GetWorkingCalendarForRequestCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                TypeId = param.TypeId,
                RequestId = param.RequestId
            };
            return changeShiftRepository.GetWorkingCalendarForRequest(criteria);
        }

        public ChangeShiftRequest GetById(long requestId)
        {
            var criteria = new GetChangeShiftRequestByIdCriteria
            {
                RequestId = requestId
            };
            return changeShiftRepository.GetById(criteria);
        }

        public IEnumerable<ChangeShiftDetail> GetDetailById(long requestId)
        {
            var criteria = new GetChangeShiftRequestDetailByIdCriteria
            {
                RequestId = requestId
            };
            return baseService.GetData<ChangeShiftDetail>(criteria);
        }

        public CommandResult Create(int actorId, CreateChangeShift model)
        {
            var criteria = new CreateChangeShiftRequestCriteria
            {
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TypeId = model.TypeId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                Remark = model.Remark,
                DetailXml = model.Details.ToXML().ToString(),
                ActorId = actorId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);
            commandResult.OutputId = criteria.RequestId;

            return commandResult;
        }

        public CommandResult Edit(int actorId, EditChangeShift model)
        {
            var criteria = new EditChangeShiftRequestCriteria
            {
                RequestId = model.RequestId,
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TypeId = model.TypeId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                Remark = model.Remark,
                DetailXml = model.Details.ToXML().ToString(),
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteChangeShiftRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailAfterCreateRequest(long requestId)
        {
            ChangeShiftRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                EmailModel email = requestMailService.GetMailToAndMailCC(requestId, (int)SettingsSendMail.New);
                var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
                email.Subject = string.Format(
                    "{0} {1} {2}",
                    Messages.ChangeShiftEmailSubject,
                    request.Employee.DisplayName,
                    forDate);

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.NickName,
                    forDate,
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

        public void SendMailAfterEditRequest(long requestId)
        {
            ChangeShiftRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, (int)SettingsSendMail.Edit);
                var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
                email.Subject = string.Format(
                    "{0} {1} {2}",
                    Messages.UpdatedChangeShiftEmailSubject,
                    request.Employee.DisplayName,
                    forDate);

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.NickName,
                    forDate,
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

        public void SendMailAfterDeleteRequest(long requestId)
        {
            ChangeShiftRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, (int)SettingsSendMail.Cancel);
                var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

                email.Subject = string.Format(Messages.MessageChangeShiftRequest, request.Employee.NickName);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.DisplayName,
                    forDate,
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