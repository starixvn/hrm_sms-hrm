// --------------------------------------------------------------------------
// <copyright file="LeaveRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.11">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
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
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// LeaveRequest Service.
    /// </summary>
    public class LeaveRequestService : RequestService, ILeaveRequestService
    {
        private const string _LeaveRequestEmailTpl = "~/app_data/emails/Leave.Request.cshtml";
        private const string _ReeditReasonEmailTpl = "~/app_data/emails/ReeditRequest.cshtml";

        /// <summary>
        /// Function to insert leave request.
        /// </summary>
        /// <param name="criteria">InsertLeaveRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Insert(InsertLeaveRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = null;
            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
                if (commandResult.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
                    var taskSendMail = new Task(() => this.SendMailForInsertLeaveSuccessfully(criteria, templateEmailPath));
                    taskSendMail.Start();
                }
            }

            return commandResult;
        }

        /// <summary>
        /// Function to update leave request.
        /// </summary>
        /// <param name="criteria">UpdateLeaveRequest Criteria.</param>
        /// <returns>Result of update.</returns>
        public CommandResult Update(UpdateLeaveRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = null;
            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
                if (commandResult.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
                    var taskSendMail = new Task(() => this.SendMailForUpdateLeaveSuccessfully(criteria, templateEmailPath));
                    taskSendMail.Start();
                }
            }

            return commandResult;
        }

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

        /// <summary>
        /// Function to reset request and send mail to supervisor in case reedit.
        /// </summary>
        /// <param name="criteria">ResetLeaveRequest Criteria.</param>
        /// <returns>Command Result.</returns>
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

        /// <summary>
        /// Function to send mail for saved success for leave request.
        /// </summary>
        /// <param name="criteria">InsertLeaveRequest Criteria.</param>
        /// <param name="templateEmailPath">Path of template email.</param>
        private void SendMailForInsertLeaveSuccessfully(InsertLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel emailModel = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);
            if (!string.IsNullOrWhiteSpace(emailModel.SendTo))
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

                emailModel.Subject = subject;
                string forDate = GetStringDateForLeaveRequest(criteria.FromDate, criteria.ToDate, criteria.TotalDays);
                var content = new
                {
                    Leader = emailModel.SendToNickName,
                    Member = criteria.EmployeeName,
                    RequestId = criteria.RequestId,
                    ForDate = forDate,
                    Remark = criteria.Remark,
                    LeaveType = criteria.LeaveTypeName,
                    Url = this.BaseUrl
                };
                emailModel.Body = Utilities.ReadTextFile(templateEmailPath);
                emailModel.Body = RazorEngine.Razor.Parse(emailModel.Body, content);

                this._EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                emailModel.SendTo,
                emailModel.SendCc,
                emailModel.Subject,
                emailModel.Body);
            }
        }

        /// <summary>
        /// Function to send mail for saved success for leave request.
        /// </summary>
        /// <param name="criteria">InsertLeaveRequest Criteria.</param>
        /// <param name="templateEmailPath">Path of template email.</param>
        private void SendMailForUpdateLeaveSuccessfully(UpdateLeaveRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel emailModel = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);
            if (!string.IsNullOrWhiteSpace(emailModel.SendTo))
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

                emailModel.Subject = subject;
                string forDate = GetStringDateForLeaveRequest(criteria.FromDate, criteria.ToDate, criteria.TotalDays);
                var content = new
                {
                    Leader = emailModel.SendToNickName,
                    Member = criteria.EmployeeName,
                    RequestId = criteria.RequestId,
                    ForDate = forDate,
                    Remark = criteria.Remark,
                    LeaveType = criteria.LeaveTypeName,
                    Url = this.BaseUrl
                };
                emailModel.Body = Utilities.ReadTextFile(templateEmailPath);
                emailModel.Body = RazorEngine.Razor.Parse(emailModel.Body, content);

                this._EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                emailModel.SendTo,
                emailModel.SendCc,
                emailModel.Subject,
                emailModel.Body);
            }
        }

        /// <summary>
        /// Function to send mail for leave request reset success.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="reason">Reason to reedit.</param>
        /// <param name="templateEmailPath">Path of template email.</param>
        private void SendMailForReSetSuccessfully(long requestId, string reason, string templateEmailPath)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = requestId
            };
            EmailModel emailModel = GetMailToAndMailCC(requestId, (int)SettingsSendMail.Reedit);
            if (!string.IsNullOrWhiteSpace(emailModel.SendTo))
            {
                var leaveRequest = GetRequestById(criteria);
                this.SendMailForReeditRequest(leaveRequest, reason, emailModel.SendToNickName, templateEmailPath, emailModel.SendTo, emailModel.SendCc);
            }
        }

        /// <summary>
        /// Function to send mail to supervisor for reedit action.
        /// </summary>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <param name="reason">The reason to reedit.</param>
        /// <param name="sendToNickName">Nick name of person email send to.</param>
        /// <param name="templateEmailPath">The path of template email.</param>
        /// <param name="mailTo">Input mail to.</param>
        /// <param name="mailCC">Input mail cc.</param>
        private void SendMailForReeditRequest(RequestEntity model, string reason, string sendToNickName, string templateEmailPath, string mailTo, string mailCC)
        {
            // send email(s)
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

            var content = new
            {
                RefNo = model.RefNO,
                Leader = sendToNickName,
                Member = model.Requester.NickName,
                Reason = reason,
                Url = this.BaseUrl
            };

            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                mailTo,
                mailCC,
                subject,
                body);
        }
    }
}
