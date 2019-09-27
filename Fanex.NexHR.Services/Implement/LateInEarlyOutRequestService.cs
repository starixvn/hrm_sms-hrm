namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Repositories;
    using Hangfire;
    using Models;
    using Models.Request;
    using Resources;

    public class LateInEarlyOutRequestService : ILateInEarlyOutRequestService
    {
        private const string LateInEarlyOutEmailTpl = "~/app_data/emails/LateInEarlyOut.Request.cshtml";
        private const string EmailTemplatePathForLateInEarlyOutCancelation = "~/app_data/emails/LateInEarlyOutCancelation.cshtml";
        private readonly IEmployeeRepository employeeRepository;
        private readonly ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly IBaseService baseService;

        public LateInEarlyOutRequestService(
            IEmployeeRepository employeeRepository,
            ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository,
            IRequestMailService requestMailService,
            IEmailService emailService,
            IBaseService baseService)
        {
            this.employeeRepository = employeeRepository;
            this.lateInEarlyOutRequestRepository = lateInEarlyOutRequestRepository;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
            this.baseService = baseService;
        }

        public IList<LateInEarlyOutModel> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria)
        {
            IList<LateInEarlyOutModel> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = lateInEarlyOutRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<LateInEarlyOutModel>();
        }

        public LateInEarlyOutModel GetById(long requestId)
        {
            var criteria = new GetLateInEarlyOutRequestByIdCriteria
            {
                RequestId = requestId
            };
            return lateInEarlyOutRequestRepository.GetById(criteria);
        }

        public IList<LateInEarlyOutModel> GetHistoryById(long requestId)
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

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteLateInEarlyOutRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(LateInEarlyOutEmailTpl);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(LateInEarlyOutEmailTpl);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.LateInEarlyOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                criteria.Remark,
                ConfigSettings.ApplicationDomain);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.UpdatedLateInEarlyOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                criteria.Remark,
                ConfigSettings.ApplicationDomain);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForDeleteSuccessfully(long requestId)
        {
            LateInEarlyOutModel request = GetById(requestId);
            if (request != null)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForLateInEarlyOutCancelation);
                BackgroundJob.Enqueue(() => SendMailForeleteRequest(request, templateEmailPath));
            }
        }

        public void SendMailForeleteRequest(LateInEarlyOutModel request, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, (int)SettingsSendMail.Cancel);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                var subject = string.Format(Messages.MessageLateInEarlyOutRequest, request.RequestTypeName, request.Employee.NickName);
                var body = Utilities.ReadTextFile(templateEmailPath);
                body = string.Format(
                    body,
                   email.SendToNickName,
                    string.Format("{0} ({1})", request.Employee.NickName, request.Employee.FullName),
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
                    request.Remark);

                emailService.SendEmail(email.Sender, email.SendTo, email.SendCC, subject, body);
            }
        }
    }
}