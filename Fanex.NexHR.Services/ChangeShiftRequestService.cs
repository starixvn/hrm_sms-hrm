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

        public IList<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(GetChangeShiftDetailToIgnoreByIdCriteria criteria)
        {
            List<ChangeShiftDetailModel> details = GetData<ChangeShiftDetailModel>(criteria).ToList();
            return details;
        }

        public void SendMailForInsertSuccessfully(InsertChangeShiftRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(ChangeShiftTmp);
            var taskSendMail = new Task(() => this.SendMailForInsertSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        public void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(ChangeShiftTmp);
            var taskSendMail = new Task(() => this.SendMailForUpdateSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        private void SendMailForInsertSuccessfully(InsertChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            string forDate = GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Subject = Messages.ChangeShiftEmailSubject + " " + employee.DisplayName + " " + GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark);
            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }

        private void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            string forDate = GetMessageDateString(criteria.FromDate, criteria.ToDate);

            email.Subject = Messages.UpdatedChangeShiftEmailSubject + " " + employee.DisplayName + " " + GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark);
            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }
    }
}