namespace Fanex.NexHR.Services
{
    using Common.Helper;
    using Core;
    using Core.Enums;
    using Criteria.Request;
    using Criteria.Request.Leave;
    using Hangfire;
    using Interfaces;
    using Models;
    using Models.Request;
    using Resources;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class LeaveRequestService : RequestMailService, ILeaveRequestService
    {
        private const string _LeaveRequestEmailTpl = "~/app_data/emails/Leave.Request.cshtml";

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
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);
            email.Subject = Messages.LeaveEmailSubject + " " + criteria.EmployeeName;
            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        public void SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);
            email.Subject = Messages.UpdatedLeaveEmailSubject + " " + criteria.EmployeeName;

            SendMailForSuccessfully(criteria, templateEmailPath, email);
        }

        private void SendMailForSuccessfully(BaseLeaveRequestCriteria criteria, string templateEmailPath, EmailModel email)
        {
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                email.Subject += criteria.TotalDays > 1 ? " from " + criteria.FromDate.ToDateMonthYearFormatedDate() + " to " + criteria.ToDate.ToDateMonthYearFormatedDate()
                                                  : " on " + criteria.FromDate.ToDateMonthYearFormatedDate();
                string forDate = criteria.TotalDays == (decimal)0.5
                    ? GetStringDateForLeaveRequest(criteria.FromDate, criteria.RequestDetail[0].FromTime, criteria.RequestDetail[0].ToTime, criteria.TotalDays)
                    : GetStringDateForLeaveRequest(criteria.FromDate, criteria.FromDate, criteria.ToDate, criteria.TotalDays);
                email.Body = Utilities.ReadTextFile(templateEmailPath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    criteria.EmployeeName,
                    forDate,
                    criteria.LeaveTypeName,
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
}