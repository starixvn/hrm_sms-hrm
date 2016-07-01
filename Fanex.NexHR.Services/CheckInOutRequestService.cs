namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Threading.Tasks;
    using System.Web;

    public class CheckInOutRequestService : RequestService, ICheckInOutRequestService
    {
        private const string CheckInOutTmp = "~/app_data/emails/CheckInOut.Request.cshtml";

        public void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
            var taskSendMail = new Task(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        public void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
            var taskSendMail = new Task(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        private void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.CheckInOutEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.CheckTime.ToShortTime(),
                    criteria.Remark);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }

        private void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.UpdatedCheckInOutEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.CheckTime.ToShortTime(),
                    criteria.Remark);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }
    }
}