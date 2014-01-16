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
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.NexHR.Core;
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
        private readonly LeaveRequestRepository _leaveRequestRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public LeaveRequestService()
        {
            this._leaveRequestRepository = new LeaveRequestRepository();
            this._requestRepository = new RequestRepository();
            this._employeeRepository = new EmployeeRepository();
            this._emailService = new EmailService();
        }

        public LeaveRequestService(
            LeaveRequestRepository leaveRequestRepository,
            IRequestRepository requestRepository,
            IEmployeeRepository employeeRepository,
            IEmailService emailService)
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._requestRepository = requestRepository;
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
        }

        /// <summary>
        /// Function to insert leave request.
        /// </summary>
        /// <param name="model">LeaveRequest Model.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Result of insert.</returns>
        public long InsertLeaveRequest(LeaveRequestModel model, out int errorCode, out string message)
        {
            if (model.FromDate <= model.ToDate)
            {
                var requestId = _leaveRequestRepository.Insert(model, out errorCode, out message);
                model.RequestId = requestId;

                if (errorCode == 0)
                {
                    SendMailForLeaveSuccessfully(model, requestId, isUpdate: false);
                }

                return requestId;
            }
            else
            {
                errorCode = 101;
                message = Messages.WrongDateRange;
                return -1;
            }
        }

        public bool UpdateLeaveRequest(LeaveRequestModel model, out int errorCode, out string message)
        {
            var result = _leaveRequestRepository.Update(model, out errorCode, out message);
            if (result)
            {
                SendMailForLeaveSuccessfully(model, model.RequestId, isUpdate: true);
            }

            return result;
        }

        /// <summary>
        /// Function to get leave request.
        /// </summary>
        /// <param name="role">Role of user.</param>
        /// <param name="fromDate">The From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">List group Id.</param>
        /// <param name="isDailyRequest">Is query from daily request.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="status">Status of request.</param>
        /// <param name="loginId">User login.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetLeaveRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = null;
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._leaveRequestRepository.GetRequests(role, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        public List<LeaveDetailModel> GetLeaveRequestDetailsById(long id)
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
        public LeaveRequestViewModel GetLeaveRequestById(long id)
        {
            LeaveRequestViewModel request = null;

            try
            {
                if (id > 0)
                {
                    var requestEntity = (RequestEntity)_leaveRequestRepository.GetRequestById(id);
                    if (requestEntity != null)
                    {
                        request = new LeaveRequestViewModel
                        {
                            RequestId = requestEntity.Id,
                            LeaveTypeId = requestEntity.LeaveTypeId,
                            Employee = requestEntity.Requester.NickName + " (" + requestEntity.Requester.FullName + ")",
                            DisplayName = requestEntity.Requester.FullName,
                            EmployeeId = requestEntity.Requester.Id,
                            FromDate = requestEntity.FromDate,
                            ToDate = requestEntity.ToDate,
                            TotalDays = requestEntity.TotalDays,
                            HalfDayOff = requestEntity.IsHalfDay,
                            VerifierId = requestEntity.VerifiedBy.Id,
                            ApproverId = requestEntity.ApprovedBy.Id,
                            RefNo = requestEntity.RefNO,
                            Remark = requestEntity.Remark,
                            FromTime = requestEntity.FromDate.ToString("HH:mm"),
                            ToTime = requestEntity.ToDate.ToString("HH:mm"),
                            FormattedStatus = requestEntity.Status.Name,
                            Status = requestEntity.Status.Id
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

        /// <summary>
        /// Function to get Leave request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoricLeaveRequests(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = null;
            try
            {
                entityList = (List<RequestEntity>)this._leaveRequestRepository.GetHistoryRequest(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
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
            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;
            var emailOfEmployee = string.Empty;

            this._leaveRequestRepository.ResetByUser(requestId, processBy, out errorCode, out errorMessage);
            if (errorCode == 0)
            {
                GetMailToAndMailCC(requestId, ref mailTo, ref mailCC, ref sendToNickName, ref emailOfEmployee);
                var leaveRequest = GetLeaveRequestById(requestId);

                if (!string.IsNullOrWhiteSpace(mailTo))
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_ReeditReasonEmailTpl);
                    var taskSendMail = new Task(() => this.SendMailForReeditRequest(leaveRequest, reason, sendToNickName, templateEmailPath, mailTo, mailCC));
                    taskSendMail.Start();
                }
            }
        }

        /// <summary>
        /// Function to get annual leave and keep in view balance of user.
        /// </summary>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="effectDay">Effect day.</param>
        /// <returns>Employee Annual Balance.</returns>
        public EmployeeAnnualBalance GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime effectDay)
        {
            var employeeBalance = new EmployeeAnnualBalance();
            if (employeeId > 0)
            {
                if (requestId.HasValue && requestId.Value == -1)
                {
                    requestId = null;
                }

                employeeBalance = _leaveRequestRepository.GetAnnualLeaveAndKeepInViewBalance(employeeId, requestId, effectDay);
            }

            return employeeBalance;
        }

        /// <summary>
        /// Function to send mail for saved success for leave request.
        /// </summary>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is update or insert.</param>
        private void SendMailForLeaveSuccessfully(LeaveRequestModel model, long requestId, bool isUpdate)
        {
            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;
            var emailOfEmployee = string.Empty;

            GetMailToAndMailCC(requestId, ref mailTo, ref mailCC, ref sendToNickName, ref emailOfEmployee);

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
                var taskSendMail = new Task(() => this.SendMailForLeaveRequest(model, sendToNickName, isUpdate, templateEmailPath, mailTo, mailCC));
                taskSendMail.Start();
            }
        }

        /// <summary>
        /// Function to get mail to and mail cc of request.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="mailTo">Output mail to.</param>
        /// <param name="mailCC">Output mail cc.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        /// <param name="emailOfEmployee">Email of employee made request.</param>
        private void GetMailToAndMailCC(long requestId, ref string mailTo, ref string mailCC, ref string sendToNickName, ref string emailOfEmployee)
        {
            SupervisorNameModel verifier = null;
            SupervisorNameModel approver = null;
            SupervisorNameModel employee = null;
            IList<SupervisorNameModel> listHr = null;

            emailOfEmployee = employee != null ? employee.Email : string.Empty;

            _requestRepository.GetSupervisorAndHrByRequestId(requestId, ref employee, ref verifier, ref approver, ref listHr);

            CheckMailApprover(approver, ref mailTo, ref sendToNickName);
            CheckMailVerifier(verifier, ref mailTo, ref sendToNickName);

            if (listHr != null && listHr.Any())
            {
                mailCC = string.Join(";", listHr.Select(x => x.Email != null ? x.Email : string.Empty));
            }

            GetEmailToAndCC(ref mailTo, ref mailCC, ref sendToNickName);

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                mailCC = GetEmailCC(mailCC, emailOfEmployee);
            }
        }

        /// <summary>
        /// Function to get EmailCC base Config setting.
        /// </summary>
        /// <param name="mailCC">Current mailCC.</param>
        /// <param name="emailOfEmployee">Email of employee.</param>
        /// <returns>EmailCC String.</returns>
        private string GetEmailCC(string mailCC, string emailOfEmployee)
        {
            if (!string.IsNullOrWhiteSpace(emailOfEmployee) && ConfigSettings.AllowSendMailToOwnerRequest)
            {
                if (string.IsNullOrWhiteSpace(mailCC))
                {
                    mailCC = emailOfEmployee;
                }
                else
                {
                    mailCC += ";" + emailOfEmployee;
                }
            }

            return mailCC;
        }

        /// <summary>
        /// Function to get email to and email CC with business.
        /// </summary>
        /// <param name="mailTo">Mail To in and out.</param>
        /// <param name="mailCC">Mail CC in and out.</param>
        /// <param name="sendToNickName">Nick name of person email send to.</param>
        private void GetEmailToAndCC(ref string mailTo, ref string mailCC, ref string sendToNickName)
        {
            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                if (mailTo.EndsWith(";"))
                {
                    mailTo = mailTo.Substring(0, mailTo.Length - 1);
                }
            }

            if (!string.IsNullOrWhiteSpace(mailCC))
            {
                if (mailCC.EndsWith(";"))
                {
                    mailCC = mailTo.Substring(0, mailTo.Length - 1);
                }
            }

            if (string.IsNullOrWhiteSpace(mailTo) && !string.IsNullOrWhiteSpace(mailCC))
            {
                mailTo = mailCC;
                mailCC = string.Empty;
                sendToNickName = Labels.HR;
            }
        }

        /// <summary>
        /// Function to check verifier.
        /// </summary>
        /// <param name="verifier">Verifier Object.</param>
        /// <param name="mailTo">Email to output.</param>
        /// <param name="sendToNickName">Nick name of person email send to.</param>
        private void CheckMailVerifier(SupervisorNameModel verifier, ref string mailTo, ref string sendToNickName)
        {
            if (verifier == null)
            {
                //// this.Process(Commands.Approve.EnumToString(), model.RequestId, model.EmployeeId, out errorMessage);
            }
            else
            {
                mailTo += verifier.Email + ";";
                if (string.IsNullOrWhiteSpace(sendToNickName))
                {
                    sendToNickName = verifier.NickName;
                }
            }
        }

        private void CheckMailApprover(SupervisorNameModel approver, ref string mailTo, ref string sendToNickName)
        {
            if (approver == null)
            {
            }
            else
            {
                mailTo = approver.Email + ";";
                sendToNickName = approver.NickName;
            }
        }

        /// <summary>
        /// Function to send mail for leave request.
        /// </summary>
        /// <param name="model">LeaveRequest ViewModel.</param>
        /// <param name="sendToNickName">Nick name of person email send to.</param>
        /// <param name="isUpdate">Is Update or insert.</param>
        /// <param name="templateEmailPath">Path of email template.</param>
        /// <param name="mailTo">MailTo List.</param>
        /// <param name="mailCC">MailCC List.</param>
        private void SendMailForLeaveRequest(LeaveRequestModel model, string sendToNickName, bool isUpdate, string templateEmailPath, string mailTo, string mailCC)
        {
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);

            string forDate = GetStringDateForLeaveRequest(model.FromDate, model.ToDate, model.TotalDays);
            string subject = (isUpdate ? Messages.UpdatedLeaveEmailSubject : Messages.LeaveEmailSubject) + " " + model.EmployeeName;

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
                Leader = sendToNickName,
                Member = model.EmployeeName,
                RequestId = model.RequestId,
                ForDate = forDate,
                Remark = model.Remark,
                LeaveType = model.LeaveTypeName,
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

            string forDate = GetStringDateForLeaveRequest(model.FromDate, model.ToDate, model.TotalDays);
            string subject = Messages.ReeditSubject + " " + model.DisplayName;

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
                Member = model.DisplayName,
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
