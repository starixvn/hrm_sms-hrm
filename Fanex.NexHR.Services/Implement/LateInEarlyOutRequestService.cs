namespace Fanex.NexHR.Services
{
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Repositories;
    using Hangfire;
    using Models;
    using Models.Request;
    using Resources;

    public class LateInEarlyOutRequestService : RequestMailService, ILateInEarlyOutRequestService
    {
        private const string LateInEarlyOutEmailTpl = "~/app_data/emails/LateInEarlyOut.Request.cshtml";

        public LateInEarlyOutRequestService(
            IEmployeeRepository employeeRepository,
            IRequestRepository requestRepository,
            IBaseService baseService,
            IEmailService emailService) : base(employeeRepository, requestRepository, baseService, emailService)
        {
        }

        public EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria)
        {
            return baseService.GetDataEntity<EmployeeLateInEarlyOutModel>(criteria);
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
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

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
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

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
    }
}