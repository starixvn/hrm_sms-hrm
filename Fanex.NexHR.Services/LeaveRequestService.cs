namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

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

        public CommandResult SendReasonReeditToSupervisor(ResetLeaveRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            if (result != null && result.Code == 0)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_ReeditReasonEmailTpl);
                var taskSendMail = new Task(() => this.SendMailForReSetSuccessfully(criteria.RequestId, criteria.Reason, templateEmailPath));
                taskSendMail.Start();
            }

            return result;
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

        private void SendMailForReSetSuccessfully(long requestId, string reason, string templateEmailPath)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = requestId
            };
            EmailModel email = GetMailToAndMailCC(requestId, (int)SettingsSendMail.Reedit);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                var leaveRequest = GetRequestById(criteria);
                this.SendMailForReeditRequest(leaveRequest, reason, email.SendToNickName, templateEmailPath, email.SendTo, email.SendCc);
            }
        }

        private void SendMailForReeditRequest(RequestEntity model, string reason, string sendToNickName, string templateEmailPath, string mailTo, string mailCC)
        {
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            string subject = Messages.ReeditSubject + " " + model.Requester.NickName;

            if (model.TotalDays > 1)
            {
                subject += " from " + model.FromDate.ToDateMonthYearFormatedDate() + " to " + model.ToDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                subject += " on " + model.FromDate.ToDateMonthYearFormatedDate();
            }

            var body = string.Format(
                bodyTemplate, 
                sendToNickName,
                string.Format("{0} ({1})", model.Requester.NickName, model.Requester.FullName),
                model.RefNO, 
                reason);
            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                mailTo,
                mailCC,
                subject,
                body);
        }
    }
}
