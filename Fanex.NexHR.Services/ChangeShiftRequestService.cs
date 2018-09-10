namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request.ChangeShift;
    using Hangfire;
    using Interfaces;
    using Models;
    using Models.Request;
    using Resources;

    public class ChangeShiftRequestService : RequestMailService, IChangeShiftRequestService
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
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(ChangeShiftTmp);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            string forDate = RequestService.GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Subject = string.Format(
                "{0} {1} {2}",
                Messages.ChangeShiftEmailSubject,
                employee.DisplayName,
                forDate);

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark, ConfigSettings.ApplicationDomain);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            string forDate = RequestService.GetMessageDateString(criteria.FromDate, criteria.ToDate);

            email.Subject = string.Format(
                "{0} {1} {2}",
                Messages.UpdatedChangeShiftEmailSubject,
                employee.DisplayName,
                RequestService.GetMessageDateString(criteria.FromDate, criteria.ToDate));

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark, ConfigSettings.ApplicationDomain);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }
    }
}