namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Hangfire;

    public class KeepInViewRequestService : IKeepInViewRequestService
    {
        private const string _KeepInViewRequestEmailTpl = "~/app_data/emails/KeepInViewRequest.cshtml";
        private const string _EmailTemplatePathForKeepInViewCancelation = "~/app_data/emails/KeepInViewCancelation.cshtml";
        protected readonly IEmployeeRepository employeeRepository;
        protected readonly IKeepInViewRequestRepository keepInViewRequestRepository;
        private readonly IBaseService baseService;
        protected readonly IRequestMailService requestMailService;
        protected readonly IEmailService emailService;

        public KeepInViewRequestService(
            IEmployeeRepository employeeRepository,
            IKeepInViewRequestRepository keepInViewRequestRepository,
            IBaseService baseService,
            IRequestMailService requestMailService,
            IEmailService emailService)
        {
            this.employeeRepository = employeeRepository;
            this.keepInViewRequestRepository = keepInViewRequestRepository;
            this.baseService = baseService;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
        }

        public IList<KeepInViewRequest> GetByDate(GetKeepInViewRequestByDateCriteria criteria)
        {
            IList<KeepInViewRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = keepInViewRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<KeepInViewRequest>();
        }

        public KeepInViewRequest GetById(long requestId)
        {
            var criteria = new GetKeepInViewRequestByIdCriteria
            {
                RequestId = requestId
            };
            return keepInViewRequestRepository.GetById(criteria);
        }

        public IList<KeepInViewRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetKeepInViewRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return keepInViewRequestRepository.GetHistoryById(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteKeepInViewRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_KeepInViewRequestEmailTpl);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_KeepInViewRequestEmailTpl);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.InsertedKeepInViewEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.Remark,
                ConfigSettings.ApplicationDomain);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.UpdatedKeepInViewEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
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
            var request = GetById(requestId);

            if (request != null)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForKeepInViewCancelation);
                BackgroundJob.Enqueue(() => SendMailForeleteRequest(request, templateEmailPath));
            }
        }

        public void SendMailForeleteRequest(KeepInViewRequest request, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, (int)SettingsSendMail.Cancel);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                var subject = string.Format(Messages.MessageKeepInViewRequest, request.Employee.NickName);
                var body = Utilities.ReadTextFile(templateEmailPath);
                body = string.Format(
                    body,
                    email.SendToNickName,
                    string.Format("{0} ({1})", request.Employee.NickName, request.Employee.FullName),
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.Remark);

                emailService.SendEmail(email.Sender, email.SendTo, email.SendCC, subject, body);
            }
        }
    }
}