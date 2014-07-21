// --------------------------------------------------------------------------
// <copyright file="RequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// RequestService Class.
    /// </summary>
    public class RequestService : IRequestService
    {
        #region constant
        private const string _MsgLeave = "<span class='req leave'>{1}</span> {0} days";
        private const string _MsgLeaveHalfDay = "<span class='req leave'>{0} ({1}-{2})</span> 0.5 day";
        private const string _MsgLeaveOneDay = "<span class='req leave'>{0}</span> 1 day";
        private const string _MsgGeneralLeaveHalfDay = "<span class='req leave'>{0}</span> 0.5 day";
        private const string _MsgLeaveDuringWorking = "<span class='req leave'>{0} ({1}-{2})</span> {3} minutes";
        private const string _MsgChangeShift = "<span class='req changeshift'>{0} <span class='arr'>&rarr;</span> {1}</span> {2}";
        private const string _MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";
        private const string _MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2}";
        private const string _MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2}";
        private const string _MsgOvertimeHours = "<span class='req overtime'>{0} ({1}-{2})</span> {3}' - {4}";
        private const string _MsgOvertimeShift = "<span class='req overtime'>{0} ({1})</span> {2}";
        private const string _MsgKeepInView = "<span class='req keepinview'>{0}</span> {1}";

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

        private const int _MaxMunites = 210;

        private const string _EmailTemplatePathForRequester = "~/app_data/emails/RequestApproval.cshtml";
        private const string _EmailTemplatePathForCheckInOut = "~/app_data/emails/CheckInOutApproval.cshtml";
        private const string _EmailTemplatePathForLateInEarlyOut = "~/app_data/emails/LateInEarlyOutApproval.cshtml";
        private const string _EmailTemplatePathForChangeShift = "~/app_data/emails/ChangeShiftApproval.cshtml";
        private const string _EmailTemplatePathForOvertime = "~/app_data/emails/OvertimeApproval.cshtml";
        private const string _EmailTemplatePathForLeave = "~/app_data/emails/LeaveApproval.cshtml";
        private const string _EmailTemplatePathForKeepInView = "~/app_data/emails/KeepInViewApproval.cshtml";
        private const string _EmailTemplatePathForLeaveCancelation = "~/app_data/emails/LeaveCancelation.cshtml";
        private const string _EmailTemplatePathForChangeShiftCancelation = "~/app_data/emails/ChangeShiftCancelation.cshtml";
        private const string _EmailTemplatePathForLateInEarlyOutCancelation = "~/app_data/emails/LateInEarlyOutCancelation.cshtml";
        private const string _EmailTemplatePathForCheckInOutCancelation = "~/app_data/emails/CheckInOutCancelation.cshtml";
        private const string _EmailTemplatePathForKeepInViewCancelation = "~/app_data/emails/KeepInViewCancelation.cshtml";

        private const string _LeaveRequestEmailTpl = "~/app_data/emails/Leave.Request.cshtml";

        #endregion

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly IShiftRepository _shiftRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly BaseRequestRepository _baseRequestRepository;
        private string _baseUrl;

        public RequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._requestRepository = new RequestRepository();
            this._emailService = new EmailService();
            this._shiftRepository = new ShiftRepository();
            this._baseRequestRepository = new BaseRequestRepository();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public RequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            IRequestRepository requestRepository,
            IShiftRepository shiftRepository,
            BaseRequestRepository baseRequestRepository)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._requestRepository = requestRepository;
            this._shiftRepository = shiftRepository;
            this._baseRequestRepository = baseRequestRepository;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public string BaseUrl
        {
            get
            {
                return this._baseUrl;
            }
        }

        /// <summary>
        /// Get info detail for request.
        /// </summary>
        /// <param name="request">RequestEntity Object.</param>
        /// <returns>Detail request.</returns>
        public static string ToRequestMessage(RequestEntity request)
        {
            StringBuilder msg = new StringBuilder();
            var message = string.Empty;

            switch (request.Type.Id)
            {
                case (int)RequestTypes.Leave:
                    message = GetMessageForLeave(request);
                    msg.Append(message);
                    break;
                case (int)RequestTypes.ChangeShift:
                    msg.AppendFormat(
                        _MsgChangeShift,
                        request.FromDate.ToDateMonthYearFormatedDate(),
                        request.ToDate.ToDateMonthYearFormatedDate(),
                        request.CreatedDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestTypes.CheckIn:
                case (int)RequestTypes.CheckOut:
                    msg.AppendFormat(_MsgCheckInOut, HourFormat(request.FromDate), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestTypes.LateIn:
                    msg.AppendFormat(_MsgLateIn, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate));
                    break;
                case (int)RequestTypes.EarlyOut:
                    msg.AppendFormat(_MsgEarlyOut, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate));
                    break;
                case (int)RequestTypes.KeepInView:
                    msg.AppendFormat(_MsgKeepInView, request.FromDate.ToDateMonthYearFormatedDate(), request.FromShift.Name);
                    break;
                default:
                    break;
            }

            return msg.ToString();
        }

        /// <summary>
        /// Get data date detail for leave request.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <returns>Date detail.</returns>
        public static string GetMessageDateString(DateTime fromDate, DateTime toDate)
        {
            string result = string.Empty;
            if ((toDate - fromDate).Days > 0)
            {
                if (fromDate.Year == toDate.Year)
                {
                    if (fromDate.Month != toDate.Month)
                    {
                        result = fromDate.ToString("dd MMM") + " - " + toDate.ToDateMonthYearFormatedDate();
                    }
                    else
                    {
                        result = fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
                    }
                }
                else
                {
                    result = fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
                }
            }
            else
            {
                result = fromDate.ToDateMonthYearFormatedDate();
            }

            return result;
        }

        /// <summary>
        /// Approve, disapprove or reject a request,
        /// notification email will be sent also.
        /// </summary>
        /// <param name="command">Action on this request.</param>
        /// <param name="requestId">Request id.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="processBy">Process By.</param>
        /// <param name="remark">Reason to proceed.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>
        /// Return boolean.
        /// </returns>
        public bool Process(string command, long requestId, int roleId, int processBy, string remark, out string errorMessage)
        {
            var request = this._requestRepository.GetWorkingTimeRequest(requestId);
            string message;
            int code = this._requestRepository.ProcessRequest(
                command, requestId, roleId, processBy, remark, DateTime.Now, out message);

            if (code == 0)
            {
                var templateEmailPathForRequester = HttpContext.Current.Server.MapPath(_EmailTemplatePathForRequester);
                var templateEmailPathForRelated = this.GetEmailTemplatePath(request.RequestType);

                var sendMailTask = new Task(() => this.SendMailProcess(command, request, processBy, templateEmailPathForRequester, templateEmailPathForRelated));
                sendMailTask.Start();
                sendMailTask.LogExceptions();
            }
            else
            {
                errorMessage = message;
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// Approve, disapprove or reject a request,
        /// notification email will be sent also.
        /// </summary>
        /// <param name="command">Action on this request.</param>
        /// <param name="requestId">Request id.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="processBy">Process By.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Return boolean.</returns>
        public bool ProcessOvertime(string command, long requestId, int roleId, int processBy, out string errorMessage)
        {
            string message;
            errorMessage = string.Empty;
            this._requestRepository.ProcessOvertime(command, requestId, roleId, processBy, DateTime.Now, out message);
            return true;
        }

        /// <summary>
        /// Approve, disapprove or reject a request,
        /// notification email will be sent also.
        /// </summary>
        /// <param name="command">Action on this request.</param>
        /// <param name="requestIds">Request ids.</param>
        /// <param name="processBy">Process By.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Return boolean.</returns>
        public bool ProcessOvertime(string command, string requestIds, int processBy, out string errorMessage)
        {
            string message;
            errorMessage = string.Empty;
            this._requestRepository.ProcessOvertime(command, requestIds, processBy, DateTime.Now, out message);
            return true;
        }

        /// <summary>
        /// Function to delete a working time request.
        /// </summary>
        /// <param name="id">WorkingTime Request Id.</param>
        /// <returns>Result of deleted.</returns>
        public bool DeleteWorkingTimeRequest(long id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            var request = this._requestRepository.GetWorkingTimeRequest(id);

            if (request != null)
            {
                var templateEmailPath = GetEmailTemplatePathForCancelRequest(request.RequestType);
                var taskSendMail = new Task(() => this.SendMailForCancelRequest(request, templateEmailPath));
                taskSendMail.Start();

                return this._requestRepository.DeleteWorkingTimeRequest(id);
            }

            return false;
        }

        /// <summary>
        /// Gets the working time requests.
        /// </summary>
        /// <param name="requestViewType">Type of the request view.</param>
        /// <param name="requestType">Request Type.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">Group Ids.</param>
        /// <param name="statusId">Status Id.</param>
        /// <param name="isDailyRequest">If set to <c>true</c> [is daily request].</param>
        /// <param name="employeeId">The employee is searched.</param>
        /// <param name="loginId">The login identifier.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>List of request.</returns>
        public IList<RequestEntity> GetRequests(
            RequestViewType requestViewType,
            int requestType,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            int statusId,
            bool isDailyRequest = false,
            int? employeeId = null,
            int? loginId = null,
            byte? isLocal = null)
        {
            if (fromDate > toDate)
            {
                return null;
            }

            IList<RequestEntity> entityList;
            string statusIds;
            if (statusId == -1)
            {
                // status = -1 --> pending and verified (1,2)
                statusIds = "1,2"; 
            }
            else if (statusId == 0)
            {
                statusIds = string.Empty;
            }
            else
            {
                statusIds = statusId.ToString();
            }

            if (employeeId.HasValue && employeeId.Value == -1)
            {
                employeeId = null;
            }

            string role = CheckRoleAccordingToRequestType(requestViewType);
            entityList = _requestRepository.GetRequests(requestType, role, fromDate, toDate, groupIds, statusIds, isDailyRequest, employeeId, loginId, isLocal);

            return entityList;
        }

        /// <summary>
        /// Function to get number of request in date range.
        /// </summary>
        /// <param name="requestType">Request Type.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <returns>Numbers of request.</returns>
        public decimal GetNumberOfRequest(int requestType, int employeeId, DateTime fromDate, DateTime toDate)
        {
            decimal numberDays = 0;
            string role = ((int)RolesEnum.Member).ToString();
            var requests = _requestRepository.GetRequests(requestType, role, fromDate, toDate, string.Empty, string.Empty, true, employeeId, null, null);

            var requestsValid = requests.Where(
                t => (t.Status.Id != (byte)CurrentRequestStatus.NotVerified 
                    && t.Status.Id != (byte)CurrentRequestStatus.Rejected
                    && t.Status.Id != (byte)CurrentRequestStatus.Void));

            if (requestsValid != null && requestsValid.Any())
            {
                if (requestType == (int)RequestFilter.Leave)
                {
                    foreach (RequestEntity request in requestsValid)
                    {
                        numberDays += request.TotalDays;
                    }
                }
                else
                {
                    numberDays = requestsValid.Count();
                }
            }
            
            return numberDays;
        }

        /// <summary>
        /// Function to get Change shift request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoryRequest(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = null;
            try
            {
                entityList = this._baseRequestRepository.GetHistoryRequest(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        public IList<SupervisorNameModel> GetSupervisorByQuery(int empId, int role, int? requestType, string query, bool includeLoginUser)
        {
            var supervisors = this._requestRepository.GetSupervisorByQuery(empId, role, requestType, query, includeLoginUser);

            return supervisors;
        }

        /// <summary>
        /// Function to get mail to and mail cc of request.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="action">Action setting send mail.</param>
        /// <param name="mailTo">Output mail to.</param>
        /// <param name="mailCC">Output mail cc.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        /// <param name="isSendForRequester">Send mail to requester or no.</param>
        /// <returns>Setting allow send to requester or no.</returns>
        public bool GetMailToAndMailCC(
            long requestId, 
            int action, 
            ref string mailTo, 
            ref string mailCC, 
            ref string sendToNickName,
            bool isSendForRequester = true)
        {
            SupervisorNameModel verifier = null;
            SupervisorNameModel approver = null;
            SupervisorNameModel employee = null;
            IList<SupervisorNameModel> listHr = null;
            EmailSettingsModel emailSettings = null;

            _requestRepository.GetSupervisorAndHrByRequestId(requestId, action, ref employee, ref verifier, ref approver, ref listHr, ref emailSettings);

            if (emailSettings.AllowSendApprover)
            {
                GetMailApprover(approver, ref mailTo, ref sendToNickName);
            }

            if (emailSettings.AllowSendVerifier 
                && !(approver != null && verifier != null
                && !string.IsNullOrWhiteSpace(verifier.Email) && !string.IsNullOrWhiteSpace(approver.Email)
                && verifier.Email.Equals(approver.Email, StringComparison.OrdinalIgnoreCase)))
            {
                GetMailVerifier(verifier, ref mailTo, ref sendToNickName);
            }

            if (emailSettings.AllowSendHR)
            {
                if (verifier != null)
                {
                    listHr = listHr.Where(t => t.Email != verifier.Email).ToList();
                }

                if (approver != null)
                {
                    listHr = listHr.Where(t => t.Email != approver.Email).ToList();
                }
                
                GetMailHR(listHr, ref mailTo, ref mailCC, ref sendToNickName);
            }

            if (isSendForRequester && emailSettings.AllowSendUser)
            {
                GetMailOwnerRequest(employee, ref mailTo, ref mailCC, ref sendToNickName);
            }

            if (emailSettings.AllowSendMailGroup)
            {
                GetMailGroup(employee, ref mailTo, ref mailCC, ref sendToNickName);
            }

            return emailSettings.AllowSendUser;
        }

        /// <summary>
        /// Function to get Hour format from date.
        /// </summary>
        /// <param name="date">Input a date.</param>
        /// <returns>String format of date.</returns>
        protected static string HourFormat(DateTime date)
        {
            return date.ToString("HH:mm");
        }

        /// <summary>
        /// Function to get string date for leave request.
        /// </summary>
        /// <param name="fromDate">The From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <param name="totalDays">Total days.</param>
        /// <returns>String detail date.</returns>
        protected string GetStringDateForLeaveRequest(DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            var forDate = string.Empty;
            if (totalDays > 1)
            {
                forDate = string.Format(_MailContentLeave, fromDate.ToDateMonthYearFormatedDate(), toDate.ToDateMonthYearFormatedDate(), totalDays);
            }
            else
            {
                if (totalDays == 1)
                {
                    forDate = string.Format(_MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
                }
                else
                {
                    forDate = string.Format(_MailContentLeaveHalfDay, fromDate.ToDateMonthYearFormatedDate(), HourFormat(fromDate), HourFormat(toDate));
                }
            }

            return forDate;
        }

        /// <summary>
        /// Function to get message for leave request.
        /// </summary>
        /// <param name="request">Request Entity.</param>
        /// <returns>Return detail of leave request.</returns>
        private static string GetMessageForLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;
            var leaveDate = GetMessageDateString(request.FromDate, request.ToDate);

            if (request.TotalDays > 1)
            {
                messageDetail = string.Format(_MsgLeave, request.TotalDays, leaveDate);
            }
            else if (request.TotalDays == 1)
            {
                messageDetail = string.Format(_MsgLeaveOneDay, GetMessageDateString(request.FromDate, request.ToDate));
            }
            else if ((request.TotalDays - (decimal)0.5) == 0)
            {
                messageDetail = string.Format(
                _MsgLeaveHalfDay, GetMessageDateString(request.FromDate, request.ToDate), HourFormat(request.FromTime), HourFormat(request.ToTime));
            }
            else
            {
                messageDetail = string.Format(
                    _MsgLeaveDuringWorking, leaveDate, HourFormat(request.FromDate), HourFormat(request.ToDate), (request.ToDate - request.FromDate).TotalMinutes);
            }

            return messageDetail;
        }

        /// <summary>
        /// Sends the mail process.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="request">The request.</param>
        /// <param name="processBy">The process user.</param>
        /// <param name="templateEmailPathForRequester">The template email path for requester.</param>
        /// <param name="templateEmailPathForRelated">The template email path for send to related people.</param>
        private void SendMailProcess(
            string command,
            WorkingTimeRequest request,
            int processBy,
            string templateEmailPathForRequester,
            string templateEmailPathForRelated)
        {
            var employee = this._employeeRepository.GetEmployeeById(request.RequesterId);
            var processUser = this._employeeRepository.GetEmployeeById(processBy);
            var action = GetActionProcessRequest(request, command);
            command = this.GetPassiveNameOfCommand(command);

            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;

            var allowSendRequester = GetMailToAndMailCC(request.RequestId, action, ref mailTo, ref mailCC, ref sendToNickName, isSendForRequester: false);

            if (allowSendRequester)
            {
                this.SendToRequester(command, request, employee, processUser, templateEmailPathForRequester);
            }

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                this.SendToRelatedPeople(command, processUser, request, employee, sendToNickName, mailTo, mailCC, templateEmailPathForRelated);
            }
        }

        /// <summary>
        /// Function ro get action setting send mail for process request.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="command">Process of request.</param>
        /// <returns>Action setting send mail.</returns>
        private int GetActionProcessRequest(WorkingTimeRequest request, string command)
        {
            var action = (int)SettingsSendMail.New;
            if (command == Commands.Accept.ToString())
            {
                action = (int)SettingsSendMail.Verify;
            }

            if (command == Commands.NotAccept.ToString())
            {
                action = (int)SettingsSendMail.NotVerify;
            }

            if (command == Commands.Approve.ToString())
            {
                action = (int)SettingsSendMail.Approve;
            }

            if (command == Commands.Reject.ToString())
            {
                action = (int)SettingsSendMail.Reject;
            }

            if (command == Commands.Process.ToString())
            {
                action = (int)SettingsSendMail.Process;
            }

            if (command == Commands.Void.ToString())
            {
                action = (int)SettingsSendMail.Void;
            }

            if (command == Commands.Reset.ToString())
            {
                if (request.Status == (byte)CurrentRequestStatus.Verified || request.Status == (byte)CurrentRequestStatus.NotVerified)
                {
                    action = (int)SettingsSendMail.Reset1;
                }
                else if (request.Status == (byte)CurrentRequestStatus.Approved || request.Status == (byte)CurrentRequestStatus.Rejected)
                {
                    action = (int)SettingsSendMail.Reset2;
                }
                else 
                {
                    action = (int)SettingsSendMail.Reset3;
                }
            }

            return action;
        }

        /// <summary>
        /// Function to send mail to requester.
        /// </summary>
        /// <param name="command">Type of changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which change status request.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendToRequester(string command, WorkingTimeRequest request, Employee employee, Employee processUser, string templateEmailPath)
        {
            string forDate = string.Empty;
            if (request.TotalDays > 1)
            {
                forDate = " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " + request.ToDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                forDate = " on " + (request.RequestType == (short)RequestTypes.KeepInView ? 
                    request.OnDate.ToDateMonthYearFormatedDate() 
                    : request.FromDate.ToDateMonthYearFormatedDate());
            }

            var bodyContent = new
            {
                Employee = employee.NickName,
                RequestId = request.RequestId,
                Action = command,
                ForDate = forDate,
                RequestType = request.RequestType.RequestTypeToText(),
                ProcessUser = processUser.NickName,
                DisplayNameOfProcessUser = processUser.FullName,
                Url = this._baseUrl
            };

            var body = Utilities.ReadTextFile(templateEmailPath);

            try
            {
                body = RazorEngine.Razor.Parse(body, bodyContent);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            var subject = string.Format(
                Resources.Messages.ProcessSubject,
                request.RequestType.RequestTypeToText(),
                forDate,
                bodyContent.Action,
                processUser.NickName);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                employee.Email,
                string.Empty,
                subject,
                body);
        }

        /// <summary>
        /// Function to get email template path when cancel a request.
        /// </summary>
        /// <param name="requestType">Request type.</param>
        /// <returns>Email template.</returns>
        private string GetEmailTemplatePathForCancelRequest(short requestType)
        {
            string templateEmailPath;

            switch (requestType)
            {
                case (short)RequestTypes.Leave:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLeaveCancelation);
                    break;
                case (short)RequestTypes.ChangeShift:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForChangeShiftCancelation);
                    break;
                case (short)RequestTypes.LateIn:
                case (short)RequestTypes.EarlyOut:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLateInEarlyOutCancelation);
                    break;
                case (short)RequestTypes.CheckIn:
                case (short)RequestTypes.CheckOut:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForCheckInOutCancelation);
                    break;
                case (short)RequestTypes.KeepInView:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForKeepInViewCancelation);
                    break;
                default:
                    templateEmailPath = string.Empty;
                    break;
            }

            return templateEmailPath;
        }

        /// <summary>
        /// Function to send mail when a request has been canceled.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="templateEmailPath">Path of email template.</param>
        private void SendMailForCancelRequest(WorkingTimeRequest request, string templateEmailPath)
        {
            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;
            var actionSendMail = (request.RequestType == (short)RequestTypes.Leave && request.ReworkCount > 0)
                ? (int)SettingsSendMail.Delete
                : (int)SettingsSendMail.Cancel;

            this.GetMailToAndMailCC(request.RequestId, actionSendMail, ref mailTo, ref mailCC, ref sendToNickName);
            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                switch (request.RequestType)
                {
                    case (short)RequestTypes.Leave:
                        this.SendMailForCancelLeaveRequest(request, mailTo, mailCC, sendToNickName, templateEmailPath);
                        break;
                    case (short)RequestTypes.ChangeShift:
                        this.SendMailForCancelChangeShiftRequest(request, mailTo, mailCC, sendToNickName, templateEmailPath);
                        break;
                    case (short)RequestTypes.LateIn:
                    case (short)RequestTypes.EarlyOut:
                        this.SendMailForCancelLateInEarlyOutRequest(request, mailTo, mailCC, sendToNickName, templateEmailPath);
                        break;
                    case (short)RequestTypes.CheckIn:
                    case (short)RequestTypes.CheckOut: 
                        this.SendMailForCancelCheckInOutRequest(request, mailTo, mailCC, sendToNickName, templateEmailPath);
                        break;
                    case (short)RequestTypes.KeepInView:
                        this.SendMailForCancelKeepInViewRequest(request, mailTo, mailCC, sendToNickName, templateEmailPath);
                        break;
                }
            }
        }

        /// <summary>
        /// Function to send mail when cancel a leave request.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendMailForCancelLeaveRequest(
            WorkingTimeRequest request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageLeaveRequestCanceled, request.Requester);
            var toDate = request.ToDate.HasValue ? request.ToDate.Value : request.FromDate;
            var forDate = GetStringDateForLeaveRequest(request.FromDate, toDate, request.TotalDays.HasValue ? request.TotalDays.Value : 0);

            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                Employee = string.Format("{0} ({1})", request.Requester, request.FullNameRequester),
                ForDate = forDate,
                Remark = request.Remark,
                LeaveName = request.LeaveName,
            };

            var body = Utilities.ReadTextFile(templateEmailPath);
            body = RazorEngine.Razor.Parse(body, bodyContent);
            
            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail when cancel a change shift request.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendMailForCancelChangeShiftRequest(
            WorkingTimeRequest request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageChangeShiftRequest, request.Requester);
            var toDate = request.ToDate.HasValue ? request.ToDate.Value : request.FromDate;
            var forDate = GetMessageDateString(request.FromDate, toDate);
            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                Employee = string.Format("{0} ({1})", request.Requester, request.FullNameRequester),
                ForDate = forDate,
                Remark = request.Remark
            };

            var body = Utilities.ReadTextFile(templateEmailPath);
            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail when cancel a(an) late in, early out request.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendMailForCancelLateInEarlyOutRequest(
            WorkingTimeRequest request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var requestType = request.RequestType == (short)RequestTypes.LateIn
                ? Resources.Labels.LateIn
                : Resources.Labels.EarlyOut;

            var subject = string.Format(Messages.MessageLateInEarlyOutRequest, requestType, request.Requester);
            var forDate = GetStringDateForLateInEarlyOutRequest(request.FromDate, request.ToDate.Value);
            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                Employee = string.Format("{0} ({1})", request.Requester, request.FullNameRequester),
                ForDate = forDate,
                RequestType = requestType,
                Minute = request.RequestType == (short)RequestTypes.LateIn ? ("+" + request.Time) : ("-" + request.Time),
                Remark = request.Remark
            };

            var body = Utilities.ReadTextFile(templateEmailPath);
            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail when cancel a check in/out request.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendMailForCancelCheckInOutRequest(
            WorkingTimeRequest request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var requestType = request.RequestType == (short)RequestTypes.CheckIn
                ? Resources.Labels.CheckIn
                : Resources.Labels.CheckOut;

            var subject = string.Format(Messages.MessageCheckInOutRequest, requestType, request.Requester);

            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                Employee = string.Format("{0} ({1})", request.Requester, request.FullNameRequester),
                RequestType = request.RequestType.RequestTypeToText().ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                Time = HourFormat(request.RequestTime),
                Shift = request.ShiftName,
                Remark = request.Remark
            };

            var body = Utilities.ReadTextFile(templateEmailPath);
            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail when cancel a keep in view request.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendMailForCancelKeepInViewRequest(
            WorkingTimeRequest request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageKeepInViewRequest, request.Requester);
            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                Employee = string.Format("{0} ({1})", request.Requester, request.FullNameRequester),
                Date = request.OnDate.ToDateMonthYearFormatedDate(),
                Shift = request.ShiftName,
                Remark = request.Remark
            };

            var body = Utilities.ReadTextFile(templateEmailPath);
            body = RazorEngine.Razor.Parse(body, bodyContent);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Method check Role.
        /// </summary>
        /// <param name="requestViewType">RequestViewType model.</param>
        /// <returns>Role after check.</returns>
        private string CheckRoleAccordingToRequestType(RequestViewType requestViewType)
        {
            var role = string.Empty;
            if (requestViewType == RequestViewType.PersonalSpace)
            {
                role = ((int)RolesEnum.Verifier).ToString() + "," + ((int)RolesEnum.Approver).ToString();
            }
            else if (requestViewType == RequestViewType.HrSpace)
            {
                role = ((int)RolesEnum.HR).ToString();
            }
            else
            {
                role = ((int)RolesEnum.Member).ToString();
            }

            return role;
        }

        /// <summary>
        /// Gets the passive name of command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The string of the passive name.</returns>
        private string GetPassiveNameOfCommand(string command)
        {
            if (command == Commands.Accept.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Accepted.ToLower());
            }

            if (command == Commands.NotAccept.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Accepted.ToLower());
            }

            if (command == Commands.Approve.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Approved.ToLower());
            }

            if (command == Commands.Reject.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Rejected.ToLower());
            }

            if (command == Commands.Process.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Processed.ToLower());
            }

            if (command == Commands.Void.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Void.ToLower());
            }

            if (command == Commands.Reset.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Reseted.ToLower());
            }

            return string.Empty;
        }

        private void GetMailApprover(SupervisorNameModel approver, ref string mailTo, ref string sendToNickName)
        {
            if (approver != null)
            {
                mailTo = approver.Email;
                sendToNickName = approver.NickName;
            }
        }

        /// <summary>
        /// Function to check verifier.
        /// </summary>
        /// <param name="verifier">Verifier Object.</param>
        /// <param name="mailTo">Email to output.</param>
        /// <param name="sendToNickName">Nick name of person email send to.</param>
        private void GetMailVerifier(SupervisorNameModel verifier, ref string mailTo, ref string sendToNickName)
        {
            if (verifier != null)
            {
                if (string.IsNullOrWhiteSpace(mailTo))
                {
                    mailTo = verifier.Email;
                    sendToNickName = verifier.NickName;
                }
                else
                {
                    mailTo += ";" + verifier.Email;
                }
            }
        }

        /// <summary>
        /// Function to get all emails of HR.
        /// </summary>
        /// <param name="listHr">Emails of HR.</param>
        /// <param name="mailTo">Output mail to.</param>
        /// <param name="mailCC">Output mail cc.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        private void GetMailHR(IList<SupervisorNameModel> listHr, ref string mailTo, ref string mailCC, ref string sendToNickName)
        {
            if (listHr != null && listHr.Any())
            {
                var mailHRs = string.Join(";", listHr.Select(x => x.Email != null ? x.Email : string.Empty));
                if (string.IsNullOrWhiteSpace(mailTo))
                {
                    mailTo = mailHRs;
                    sendToNickName = Labels.HR;
                }
                else
                {
                    mailCC = mailHRs;
                }
            }
        }

        /// <summary>
        /// Function to get email of Owner request.
        /// </summary>
        /// <param name="employee">Employee which create request.</param>
        /// <param name="mailTo">Output mail to.</param>
        /// <param name="mailCC">Output mail cc.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        private void GetMailOwnerRequest(SupervisorNameModel employee, ref string mailTo, ref string mailCC, ref string sendToNickName)
        {
            if (employee != null)
            {
                if (string.IsNullOrWhiteSpace(mailTo))
                {
                    mailTo = employee.Email;
                    sendToNickName = employee.NickName;
                }
                else if (string.IsNullOrWhiteSpace(mailCC))
                {
                    mailCC = employee.Email;
                }
                else
                {
                    mailCC += ";" + employee.Email;
                }
            }
        }

        /// <summary>
        /// Function to get email group.
        /// </summary>
        /// <param name="employee">Employee which create request.</param>
        /// <param name="mailTo">Output mail to.</param>
        /// <param name="mailCC">Output mail cc.</param>
        /// <param name="sendToNickName">Nick name which this request will be sent.</param>
        private void GetMailGroup(SupervisorNameModel employee, ref string mailTo, ref string mailCC, ref string sendToNickName)
        {
            if (employee != null)
            {
                if (string.IsNullOrWhiteSpace(mailTo))
                {
                    mailTo = employee.MailGroup;
                    sendToNickName = Labels.All;
                }
                else if (string.IsNullOrWhiteSpace(mailCC))
                {
                    mailCC = employee.MailGroup;
                }
                else
                {
                    mailCC += ";" + employee.MailGroup;
                }
            }
        }

        /// <summary>
        /// Function to get path of email template.
        /// </summary>
        /// <param name="requestType">Request type.</param>
        /// <returns>Path of email template.</returns>
        private string GetEmailTemplatePath(short requestType)
        {
            var path = string.Empty;
            switch (requestType)
            {
                case (short)RequestTypes.CheckIn:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForCheckInOut);
                    break;
                case (short)RequestTypes.CheckOut:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForCheckInOut);
                    break;
                case (short)RequestTypes.LateIn:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLateInEarlyOut);
                    break;
                case (short)RequestTypes.EarlyOut:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLateInEarlyOut);
                    break;
                case (short)RequestTypes.ChangeShift:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForChangeShift);
                    break;
                case (short)RequestTypes.Leave:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLeave);
                    break;
                case (short)RequestTypes.Overtime:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForOvertime);
                    break;
                case (short)RequestTypes.KeepInView:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForKeepInView);
                    break;
            }

            return path;
        }

        /// <summary>
        /// Function to send mail to other staff which related group.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="sendToNickName">Nick name which mail send to.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCc">The mail cc.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void SendToRelatedPeople(
            string command,
            Employee processUser,
            WorkingTimeRequest request,
            Employee employee,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            switch (request.RequestType)
            {
                case (short)RequestTypes.CheckIn:
                    var subjectCheckIn = string.Format(Messages.ApprovalNotificationCheckInSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, subjectCheckIn, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestTypes.CheckOut:
                    var subjectCheckOut = string.Format(Messages.ApprovalNotificationCheckOutSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, subjectCheckOut, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestTypes.LateIn:
                    var subjectLateIn = string.Format(Messages.ApprovalNotificationLateInSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, subjectLateIn, sendToNickName, mailTo, mailCc, true, templateEmailPath);
                    break;
                case (short)RequestTypes.EarlyOut:
                    var subjectEarlyOut = string.Format(Messages.ApprovalNotificationEarlyOutSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, subjectEarlyOut, sendToNickName, mailTo, mailCc, false, templateEmailPath);
                    break;
                case (short)RequestTypes.ChangeShift:
                    var subjectChangeShift = string.Format(Messages.ApprovalNotificationChangeShiftSubject, employee.NickName, command, processUser.NickName);
                    ChangeShiftRelatedMail(command, employee, processUser, request, subjectChangeShift, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestTypes.Leave:
                    var subjectLeave = string.Format(Messages.ApprovalNotificationLeaveSubject, employee.NickName, command, processUser.NickName);
                    LeaveRelatedMail(command, employee, processUser, request, subjectLeave, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestTypes.KeepInView:
                    var subjectKeepInView = string.Format(Messages.ApprovalNotificationKeepInViewSubject, employee.NickName, command, processUser.NickName);
                    KeepInViewRelatedMail(command, employee, processUser, request, subjectKeepInView, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
            }
        }

        /// <summary>
        /// Function to send mail check in out request to other staff which related group.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendToNickName">Nick name which mail send to.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void CheckInOutRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            WorkingTimeRequest request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            var shift = this._shiftRepository.GetByDate(employee.Id, request.FromDate);
            var body = Utilities.ReadTextFile(templateEmailPath);
            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                ProcessUser = string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                RequestType = request.RequestType.RequestTypeToText().ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                Time = HourFormat(request.FromDate),
                Shift = shift != null ? shift.Name : string.Empty,
                Url = this._baseUrl,
                Action = command
            };

            try
            {
                body = RazorEngine.Razor.Parse(body, bodyContent);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                mailCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail late in, early out request to other staff which related group.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendToNickName">Nick name which mail send to.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCc">Email CC List.</param>
        /// <param name="isLateIn">Is late in or early out.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void LateInEarlyOutRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            WorkingTimeRequest request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            bool isLateIn,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = GetStringDateForLateInEarlyOutRequest(request.FromDate, request.ToDate.Value);

            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                ProcessUser = string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                RequestType = request.RequestType == (short)RequestTypes.LateIn
                                ? "a " + Resources.Labels.LateIn.ToLower()
                                : "an " + Resources.Labels.EarlyOut.ToLower(),
                ForDate = forDate,
                Minute = isLateIn ? ("+" + request.Time) : ("-" + request.Time),
                Remark = request.Remark,
                Url = this._baseUrl,
                Action = command
            };

            try
            {
                body = RazorEngine.Razor.Parse(body, bodyContent);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                mailCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail change shift request to other staff which related group.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendToNickName">Nick name which mail send to.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void ChangeShiftRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            WorkingTimeRequest request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var toDate = request.ToDate.HasValue ? request.ToDate.Value : request.FromDate;
            var forDate = GetMessageDateString(request.FromDate, toDate);
            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                ProcessUser = string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                RequestType = request.RequestType.RequestTypeToText().ToLower(),
                ForDate = forDate,
                Remark = request.Remark,
                Action = command
            };

            try
            {
                body = RazorEngine.Razor.Parse(body, bodyContent);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                mailCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail leave request to other staff which related group.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendToNickName">Nick name which mail send to.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="sendCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void LeaveRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            WorkingTimeRequest request,
            string subject,
            string sendToNickName,
            string mailTo,
            string sendCc,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);

            var toDate = request.ToDate.HasValue ? request.ToDate.Value : request.FromDate;
            var forDate = GetStringDateForLeaveRequest(request.FromDate, toDate, request.TotalDays.HasValue ? request.TotalDays.Value : 0);

            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                ProcessUser = string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                RequestType = request.RequestType.RequestTypeToText().ToLower(),
                ForDate = forDate,
                Remark = request.Remark,
                LeaveName = request.LeaveName,
                Url = this._baseUrl,
                Action = command
            };

            try
            {
                body = RazorEngine.Razor.Parse(body, bodyContent);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail keep in view request to other staff which related group.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="employee">Employee which made request.</param>
        /// <param name="processUser">Employee which changed status request.</param>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="sendToNickName">Nick name which mail send to.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCc">Email CC List.</param>
        /// <param name="templateEmailPath">Path of Email template.</param>
        private void KeepInViewRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            WorkingTimeRequest request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            var shift = this._shiftRepository.GetByDate(employee.Id, request.OnDate);
            var body = Utilities.ReadTextFile(templateEmailPath);
            var bodyContent = new
            {
                SendToNickName = sendToNickName,
                ProcessUser = string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                RequestType = request.RequestType.RequestTypeToText().ToLower(),
                Date = request.OnDate.ToDateMonthYearFormatedDate(),
                Shift = shift != null ? shift.Name : string.Empty,
                Remark = request.Remark,
                Url = this._baseUrl,
                Action = command
            };

            try
            {
                body = RazorEngine.Razor.Parse(body, bodyContent);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                mailCc,
                subject,
                body);
        }

        /// <summary>
        /// Function to get string date for late in, early out request.
        /// </summary>
        /// <param name="fromDate">The From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <returns>String detail date.</returns>
        private string GetStringDateForLateInEarlyOutRequest(DateTime fromDate, DateTime toDate)
        {
            var date = string.Empty;

            if (toDate == null || (toDate - fromDate).Days == 0)
            {
                date = fromDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                date = "from " + fromDate.ToDateMonthYearFormatedDate() + " to " + toDate.ToDateMonthYearFormatedDate();
            }

            return date;
        }
    }
}