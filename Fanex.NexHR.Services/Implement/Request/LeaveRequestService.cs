namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Hangfire;

    public class LeaveRequestService : ILeaveRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/Leave.Request.cshtml";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/Leave.Cancel.cshtml";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/Leave.Approval.cshtml";
        private readonly ILeaveRequestRepository leaveRequestRepository;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly IBaseService baseService;

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository,
            IRequestMailService requestMailService,
            IBaseService baseService,
            IEmailService emailService)
        {
            this.leaveRequestRepository = leaveRequestRepository;
            this.requestMailService = requestMailService;
            this.baseService = baseService;
            this.emailService = emailService;
        }

        public IList<LeaveRequest> GetByDate(GetLeaveRequestByDateCriteria criteria)
        {
            IList<LeaveRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = leaveRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<LeaveRequest>();
        }

        public LeaveRequest GetById(long requestId)
        {
            var criteria = new GetLeaveRequestByIdCriteria
            {
                RequestId = requestId
            };
            return leaveRequestRepository.GetById(criteria);
        }

        public IList<LeaveRequestDetail> GetLeaveRequestDetailById(long requestId)
        {
            var criteria = new GetLeaveRequestDetailByIdCriteria
            {
                RequestId = requestId
            };
            return baseService.GetData<LeaveRequestDetail>(criteria).ToList();
        }

        public IList<LeaveRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetLeaveRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return leaveRequestRepository.GetHistoryById(criteria);
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

        public EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria)
        {
            return baseService.GetDataEntity<EmployeeLeaveBalance>(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteLeaveRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailCreateRequest(InsertLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailEditRequest(UpdateLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);
            email.Subject = Messages.LeaveEmailSubject + " " + criteria.EmployeeName;
            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        public void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);
            email.Subject = Messages.UpdatedLeaveEmailSubject + " " + criteria.EmployeeName;

            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        public void SendMailDeleteRequest(long requestId)
        {
            LeaveRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                EmailModel email = requestMailService.GetMailToAndMailCC(requestId, (byte)SettingsSendMail.Cancel);
                var fromDate = request.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
                var toDate = request.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
                var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

                email.Subject = string.Format(Messages.MessageLeaveRequestCanceled, request.Employee.NickName);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.DisplayName,
                    forDate,
                    request.LeaveType.Name,
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
            LeaveRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                byte action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                string commandMessage = requestMailService.GetCommandMessage(command);
                var fromDate = request.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
                var toDate = request.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
                var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

                EmailModel email = requestMailService.GetMailToAndMailCC(requestId, action);
                email.Subject = string.Format(Messages.ApprovalNotificationChangeShiftSubject, request.Employee.NickName, commandMessage, actor.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.DisplayName,
                    commandMessage,
                    actor.DisplayName,
                    request.RequestTypeName,
                    forDate,
                    request.LeaveType.Name,
                    request.Remark);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));
            }
        }

        public DateTime GetFromTimeHalfDay(IList<LeaveRequestDetail> leaveDetails)
        {
            var fromTime = DateTime.Now;
            var leaveHalfDay = leaveDetails.FirstOrDefault(t => t.WorkType == 1 && t.IsHalfDay);
            if (leaveHalfDay != null)
            {
                fromTime = leaveHalfDay.FromTime;
            }

            return fromTime;
        }

        public DateTime GetToTimeHalfDay(IList<LeaveRequestDetail> leaveDetails)
        {
            var toTime = DateTime.Now;
            var leaveHalfDay = leaveDetails.FirstOrDefault(t => t.WorkType == 1 && t.IsHalfDay);
            if (leaveHalfDay != null)
            {
                toTime = leaveHalfDay.ToTime;
            }

            return toTime;
        }

        private void SendMailForSuccessfully(BaseLeaveRequestCriteria criteria, string templateEmailPath, EmailModel email)
        {
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                email.Subject += criteria.TotalDays > 1
                    ? " from " + criteria.FromDate.ToDateMonthYearFormatedDate() + " to " + criteria.ToDate.ToDateMonthYearFormatedDate()
                    : " on " + criteria.FromDate.ToDateMonthYearFormatedDate();
                var fromDate = criteria.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(criteria.RequestDetail) : criteria.FromDate;
                var toDate = criteria.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(criteria.RequestDetail) : criteria.ToDate;
                var forDate = requestMailService.GetStringDateForLeaveRequest(criteria.FromDate, fromDate, toDate, criteria.TotalDays);

                email.Body = Utilities.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    criteria.EmployeeName,
                    forDate,
                    criteria.LeaveTypeName,
                    criteria.Remark,
                    ConfigSettings.ApplicationDomain);

                emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body);
            }
        }
    }
}