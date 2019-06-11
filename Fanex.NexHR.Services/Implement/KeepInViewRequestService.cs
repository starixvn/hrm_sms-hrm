namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request.KeepInView;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Hangfire;
    using Models;
    using Resources;

    public class KeepInViewRequestService: IKeepInViewRequestService
    {
        private const string _KeepInViewRequestEmailTpl = "~/app_data/emails/KeepInViewRequest.cshtml";
        private const string _EmailTemplatePathForKeepInViewCancelation = "~/app_data/emails/KeepInViewCancelation.cshtml";
        protected readonly IEmployeeRepository employeeRepository;
        protected readonly IKeepInViewRequestRepository keepInViewRequestRepository;
        protected readonly IRequestMailService requestMailService;
        protected readonly IEmailService emailService;

        public KeepInViewRequestService(
            IEmployeeRepository employeeRepository,
            IKeepInViewRequestRepository keepInViewRequestRepository,
            IRequestMailService requestMailService,
            IEmailService emailService)
        {
            this.employeeRepository = employeeRepository;
            this.keepInViewRequestRepository = keepInViewRequestRepository;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
        }

        public IList<KeepInViewModel> GetByDate(GetKeepInViewRequestByDateCriteria criteria)
        {
            IList<KeepInViewModel> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = keepInViewRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<KeepInViewModel>();
        }

        public KeepInViewModel GetById(GetKeepInViewRequestByIdCriteria criteria)
        {
            KeepInViewModel request = null;
            if (criteria != null && criteria.IsValid())
            {
                request = keepInViewRequestRepository.GetById(criteria);
            }

            return request ?? new KeepInViewModel();
        }

        public IList<KeepInViewModel> GetHistoryById(GetKeepInViewHistoryByIdCriteria criteria)
        {
            IList<KeepInViewModel> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = keepInViewRequestRepository.GetHistoryById(criteria);
            }

            return requests ?? new List<KeepInViewModel>();
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
            var criteria = new GetKeepInViewRequestByIdCriteria
            {
                RequestId = requestId
            };
            var request = GetById(criteria);

            if (request != null)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForKeepInViewCancelation);
                BackgroundJob.Enqueue(() => SendMailForeleteRequest(request, templateEmailPath));
            }
        }

        public void SendMailForeleteRequest(KeepInViewModel request, string templateEmailPath)
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