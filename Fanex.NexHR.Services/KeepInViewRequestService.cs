namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Threading.Tasks;
    using System.Web;

    public class KeepInViewRequestService : RequestService, IKeepInViewRequestService
    {
        private const string _KeepInViewRequestEmailTpl = "~/app_data/emails/KeepInViewRequest.cshtml";

        public void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_KeepInViewRequestEmailTpl);
            Task.Factory.StartNew(() =>
            {
                SendMailForInsertSuccessfully(criteria, templateEmailPath);
            });
        }

        public void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_KeepInViewRequestEmailTpl);
            Task.Factory.StartNew(() =>
            {
                SendMailForUpdateSuccessfully(criteria, templateEmailPath);
            });
        }

        private void SendMailForInsertSuccessfully(InsertKeepInViewRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                EmpId = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.InsertedKeepInViewEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.ShiftName,
                criteria.Remark);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        private void SendMailForUpdateSuccessfully(UpdateKeepInViewRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                EmpId = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.UpdatedKeepInViewEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.ShiftName,
                criteria.Remark);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }
    }
}