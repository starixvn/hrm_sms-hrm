namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public class LeaveRequestService : RequestService, ILeaveRequestService
    {
        private const string _LeaveRequestEmailTpl = "~/app_data/emails/Leave.Request.cshtml";
        private const string _ReeditReasonEmailTpl = "~/app_data/emails/ReeditRequest.cshtml";

        public IList<LeaveDetailModel> GetLeaveRequestDetailById(GetLeaveRequestDetailByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var detail = new List<LeaveDetailModel>();
            if (criteria.IsValid())
            {
                detail = GetData<LeaveDetailModel>(criteria).ToList();
            }

            return detail;
        }

        public EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var employeeLeaveBalance = new EmployeeLeaveBalance();
            if (criteria.IsValid())
            {
                employeeLeaveBalance = GetDataEntity<EmployeeLeaveBalance>(criteria);
            }

            return employeeLeaveBalance;
        }

        public void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
            var taskSendMail = new Task(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        public void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
            var taskSendMail = new Task(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        private void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                string subject = Messages.LeaveEmailSubject + " " + criteria.EmployeeName;
                if (criteria.TotalDays > 1)
                {
                    subject += " from " + criteria.FromDate.ToDateMonthYearFormatedDate() + " to " + criteria.ToDate.ToDateMonthYearFormatedDate();
                }
                else
                {
                    subject += " on " + criteria.FromDate.ToDateMonthYearFormatedDate();
                }

                email.Subject = subject;
                string forDate = criteria.TotalDays == (decimal)0.5
                ? GetStringDateForLeaveRequest(criteria.FromDate, criteria.RequestDetail[0].FromTime, criteria.RequestDetail[0].ToTime, criteria.TotalDays)
                : GetStringDateForLeaveRequest(criteria.FromDate, criteria.FromDate, criteria.ToDate, criteria.TotalDays);
                email.Body = Utilities.ReadTextFile(templateEmailPath);
                email.Body = string.Format(email.Body, email.SendToNickName, criteria.EmployeeName, forDate, criteria.LeaveTypeName, criteria.Remark);

                EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
            }
        }

        private void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                string subject = Messages.UpdatedLeaveEmailSubject + " " + criteria.EmployeeName;
                if (criteria.TotalDays > 1)
                {
                    subject += " from " + criteria.FromDate.ToDateMonthYearFormatedDate() + " to " + criteria.ToDate.ToDateMonthYearFormatedDate();
                }
                else
                {
                    subject += " on " + criteria.FromDate.ToDateMonthYearFormatedDate();
                }

                email.Subject = subject;
                string forDate = criteria.TotalDays == (decimal)0.5
                    ? GetStringDateForLeaveRequest(criteria.FromDate, criteria.RequestDetail[0].FromTime, criteria.RequestDetail[0].ToTime, criteria.TotalDays)
                    : GetStringDateForLeaveRequest(criteria.FromDate, criteria.FromDate, criteria.ToDate, criteria.TotalDays);
                email.Body = Utilities.ReadTextFile(templateEmailPath);
                email.Body = string.Format(email.Body, email.SendToNickName, criteria.EmployeeName, forDate, criteria.LeaveTypeName, criteria.Remark);

                EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
            }
        }
    }
}
