namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public class ChangeShiftRequestService : RequestService, IChangeShiftRequestService
    {
        private const string ChangeShiftTmp = "~/app_data/emails/ChangeShift.Request.cshtml";

        public IList<ChangeShiftDetailModel> GetChangeShiftRequestDetailById(GetChangeShiftRequestDetailByIdCriteria criteria)
        {
            var detail = GetData<ChangeShiftDetailModel>(criteria).ToList();
            return detail;
        }

        public void SendMailForInsertSuccessfully(InsertChangeShiftRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(ChangeShiftTmp);
            Task.Factory.StartNew(() =>
            {
                SendMailForInsertSuccessfully(criteria, templateEmailPath);
            });
        }

        public void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(ChangeShiftTmp);
            Task.Factory.StartNew(() =>
            {
                SendMailForUpdateSuccessfully(criteria, templateEmailPath);
            });
        }

        private void SendMailForInsertSuccessfully(InsertChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                EmpId = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            string forDate = GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Subject = Messages.ChangeShiftEmailSubject + " " + employee.DisplayName + " " + GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark);
            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        private void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                EmpId = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            string forDate = GetMessageDateString(criteria.FromDate, criteria.ToDate);

            email.Subject = Messages.UpdatedChangeShiftEmailSubject + " " + employee.DisplayName + " " + GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark);
            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }
    }
}