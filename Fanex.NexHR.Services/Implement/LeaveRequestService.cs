namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Request.Leave;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Repositories;
    using Hangfire;
    using Models;
    using Models.Request;
    using Resources;

    public class LeaveRequestService : ILeaveRequestService
    {
        private const string _LeaveRequestEmailTpl = "~/app_data/emails/Leave.Request.cshtml";
        private const string _EmailTemplatePathForLeaveCancelation = "~/app_data/emails/LeaveCancelation.cshtml";
        protected readonly IEmployeeRepository employeeRepository;
        protected readonly IRequestRepository requestRepository;
        protected readonly ILeaveRequestRepository leaveRequestRepository;
        protected readonly IRequestMailService requestMailService;
        protected readonly IEmailService emailService;
        private readonly IBaseService baseService;

        public LeaveRequestService(
            IEmployeeRepository employeeRepository,
            IRequestRepository requestRepository,
            ILeaveRequestRepository leaveRequestRepository,
            IRequestMailService requestMailService,
            IBaseService baseService,
            IEmailService emailService)
        {
            this.employeeRepository = employeeRepository;
            this.requestRepository = requestRepository;
            this.leaveRequestRepository = leaveRequestRepository;
            this.requestMailService = requestMailService;
            this.baseService = baseService;
            this.emailService = emailService;
        }

        public IList<LeaveModel> GetByDate(GetLeaveRequestByDateCriteria criteria)
        {
            IList<LeaveModel> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = leaveRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<LeaveModel>();
        }

        public LeaveModel GetById(GetLeaveRequestByIdCriteria criteria)
        {
            LeaveModel request = null;
            if (criteria != null && criteria.IsValid())
            {
                request = leaveRequestRepository.GetById(criteria);
            }

            return request ?? new LeaveModel();
        }

        public IList<LeaveDetailModel> GetLeaveRequestDetailById(GetLeaveRequestDetailByIdCriteria criteria)
        {
            return baseService.GetData<LeaveDetailModel>(criteria).ToList();
        }

        public IList<LeaveModel> GetHistoryById(GetLeaveRequestHistoryByIdCriteria criteria)
        {
            IList<LeaveModel> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = leaveRequestRepository.GetHistoryById(criteria);
            }

            return requests ?? new List<LeaveModel>();
        }

        public EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria)
        {
            return baseService.GetDataEntity<EmployeeLeaveBalance>(criteria);
        }

        public void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
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

        public void SendMailForDeleteSuccessfully(long requestId)
        {
            var criteria = new GetLeaveRequestByIdCriteria
            {
                RequestId = requestId
            };
            LeaveModel request = leaveRequestRepository.GetById(criteria);

            if (request != null)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLeaveCancelation);
                BackgroundJob.Enqueue(() => SendMailForCancelRequest(request, templateEmailPath));
            }
        }

        public void SendMailForCancelRequest(LeaveModel request, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, (int)SettingsSendMail.Cancel);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                var subject = string.Format(Messages.MessageLeaveRequestCanceled, request.Employee.NickName);
                
                var fromDate = request.TotalDays == (decimal)0.5 ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
                var toDate = request.TotalDays == (decimal)0.5 ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
                var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

                var displayName = string.Format("{0} ({1})", request.Employee.NickName, request.Employee.FullName);
                var body = Utilities.ReadTextFile(templateEmailPath);
                body = string.Format(body, email.SendToNickName, displayName, forDate, request.LeaveType.Name, request.Remark);

                emailService.SendEmail(email.Sender, email.SendTo, email.SendCC, subject, body);
            }
        }

        public DateTime GetFromTimeHalfDay(IList<LeaveDetailModel> leaveDetails)
        {
            var fromTime = DateTime.Now;
            if (leaveDetails.Any())
            {
                var leaveHalfDay = leaveDetails.FirstOrDefault(t => (t.WorkType == 1 && t.IsHalfDay));
                fromTime = leaveHalfDay.FromTime;
            }

            return fromTime;
        }

        public DateTime GetToTimeHalfDay(IList<LeaveDetailModel> leaveDetails)
        {
            var toTime = DateTime.Now;
            if (leaveDetails.Any())
            {
                var leaveHalfDay = leaveDetails.FirstOrDefault(t => (t.WorkType == 1 && t.IsHalfDay));
                toTime = leaveHalfDay.ToTime;
            }

            return toTime;
        }

        private void SendMailForSuccessfully(BaseLeaveRequestCriteria criteria, string templateEmailPath, EmailModel email)
        {
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                email.Subject += criteria.TotalDays > 1 ? " from " + criteria.FromDate.ToDateMonthYearFormatedDate() + " to " + criteria.ToDate.ToDateMonthYearFormatedDate()
                                                  : " on " + criteria.FromDate.ToDateMonthYearFormatedDate();
                var fromDate = criteria.TotalDays == (decimal)0.5 ? GetFromTimeHalfDay(criteria.RequestDetail) : criteria.FromDate;
                var toDate = criteria.TotalDays == (decimal)0.5 ? GetToTimeHalfDay(criteria.RequestDetail) : criteria.ToDate;
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