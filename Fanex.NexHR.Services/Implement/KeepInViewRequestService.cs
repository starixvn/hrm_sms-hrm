namespace Fanex.NexHR.Services
{
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request.KeepInView;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Repositories;
    using Hangfire;
    using Models;
    using Resources;

    public class KeepInViewRequestService : RequestMailService, IKeepInViewRequestService
    {
        private const string _KeepInViewRequestEmailTpl = "~/app_data/emails/KeepInViewRequest.cshtml";

        public KeepInViewRequestService(
            IEmployeeRepository employeeRepository,
            IRequestRepository requestRepository,
            IBaseService baseService,
            IEmailService emailService) : base(employeeRepository, requestRepository, baseService, emailService)
        {
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
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

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
                criteria.ShiftName,
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
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

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
                criteria.ShiftName,
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