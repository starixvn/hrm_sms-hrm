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
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmailService _emailService;

        public LeaveRequestService()
        {
            this._leaveRequestRepository = new LeaveRequestRepository();
            this._emailService = new EmailService();
        }

        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, IEmailService emailService)
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._emailService = emailService;
        }

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

        public IList<LeaveDetailModel> GetLeaveRequestDetailsById(long id)
        {
            if (id > 0)
            {
                return this._leaveRequestRepository.GetLeaveRequestDetailsById(id);
            }

            return null;
        }

        /// <summary>
        /// Function to get Leave request by id.
        /// </summary>
        /// <param name="id">Leave request id.</param>
        /// <returns>LeaveRequest ViewModel.</returns>
        public LeaveRequestViewModel GetRequestById(long id)
        {
            LeaveRequestViewModel request = null;
            try
            {
                if (id > 0)
                {
                    var requestEntity = (RequestEntity)_leaveRequestRepository.GetRequestById(id, (int)RequestFilter.Leave);
                    if (requestEntity != null)
                    {
                        request = new LeaveRequestViewModel
                        {
                            RequestId = requestEntity.Id,
                            LeaveTypeId = requestEntity.LeaveType.Id,
                            Employee = requestEntity.Requester.DisplayName,
                            EmployeeId = requestEntity.Requester.Id,
                            FromDate = requestEntity.FromDate,
                            ToDate = requestEntity.ToDate,
                            TotalDays = requestEntity.TotalDays,
                            HalfDayOff = requestEntity.IsHalfDay,
                            VerifierId = requestEntity.VerifiedBy.Id,
                            ApproverId = requestEntity.ApprovedBy.Id,
                            RefNo = requestEntity.RefNO,
                            Remark = requestEntity.Remark,
                            FromTime = requestEntity.FromTime.ToString("HH:mm"),
                            ToTime = requestEntity.ToTime.ToString("HH:mm"),
                            IsFromTimeAfterDay = requestEntity.FromDate.Date != requestEntity.FromTime.Date,
                            IsToTimeAfterDay = requestEntity.ToDate.Date != requestEntity.ToTime.Date,
                            Status = requestEntity.Status
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return request;
        }

        public RequestEntity GetRequestById(GetRequestByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            RequestEntity request = null;
            if (criteria.IsValid())
            {
                request = GetDataEntity<RequestEntity>(criteria);
            }

            return request;
        }

        /// <summary>
        /// Function to reset request and send mail to supervisor in case reedit.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="processBy">User reedit.</param>
        /// <param name="reason">The reason of reedit.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="errorMessage">Error Message.</param>
        public void SendReasonReeditToSupervisor(long requestId, int processBy, string reason, out int errorCode, out string errorMessage)
        {
            this._leaveRequestRepository.ResetByUser(requestId, processBy, out errorCode, out errorMessage);
            if (errorCode == 0)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_ReeditReasonEmailTpl);
                var taskSendMail = new Task(() => this.SendMailForReSetSuccessfully(requestId, reason, templateEmailPath));
                taskSendMail.Start();
            }
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

                this._emailService.SendEmail(
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

                this._emailService.SendEmail(
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
            EmailModel emailModel = GetMailToAndMailCC(requestId, (int)SettingsSendMail.Reedit);
            if (!string.IsNullOrWhiteSpace(emailModel.SendTo))
            {
                var leaveRequest = GetRequestById(requestId);
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
        private void SendMailForReeditRequest(LeaveRequestViewModel model, string reason, string sendToNickName, string templateEmailPath, string mailTo, string mailCC)
        {
            // send email(s)
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            string subject = Messages.ReeditSubject + " " + model.Employee;

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
                RefNo = model.RefNo,
                Leader = sendToNickName,
                Member = model.Employee,
                Reason = reason,
                Url = this.BaseUrl
            };

            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._emailService.SendEmail(
                ConfigSettings.EmailSetting,
                mailTo,
                mailCC,
                subject,
                body);
        }
    }
}
