namespace Fanex.NexHR.Services
{
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request.CheckInOut;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Repositories;
    using Hangfire;
    using Models;
    using Resources;

    public class CheckInOutRequestService : RequestMailService, ICheckInOutRequestService
    {
        private const string CheckInOutTmp = "~/app_data/emails/CheckInOut.Request.cshtml";

        public CheckInOutRequestService(
            IEmployeeRepository employeeRepository,
            IRequestRepository requestRepository,
            IBaseService baseService,
            IEmailService emailService) : base(employeeRepository, requestRepository, baseService, emailService)
        {
        }

        public void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.CheckInOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.CheckTime.ToShortTime(),
                    criteria.Remark,
                ConfigSettings.ApplicationDomain);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.UpdatedCheckInOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.CheckTime.ToShortTime(),
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