namespace Fanex.NexHR.Services
{
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request.LateInEarlyOut;
    using Hangfire;
    using Interfaces;
    using Models;
    using Models.Request;
    using Resources;
    using System.Web;

    public class LateInEarlyOutRequestService : RequestMailService, ILateInEarlyOutRequestService
    {
        private const string LateInEarlyOutEmailTpl = "~/app_data/emails/LateInEarlyOut.Request.cshtml";

        public EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria)
        {
            EmployeeLateInEarlyOutModel lateInEarlyOut = GetDataEntity<EmployeeLateInEarlyOutModel>(criteria);
            return lateInEarlyOut;
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
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

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

            EmailService.SendEmail(
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
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

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

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }
    }
}