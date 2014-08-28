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
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// RequestService Class.
    /// </summary>
    public class RequestService : BaseService, IRequestService
    {
        #region constant
        private const string _MsgLeave = "<span class='req leave'>{1}</span> {0} days";
        private const string _MsgLeaveHalfDay = "<span class='req leave'>{0} ({1}-{2})</span> 0.5 day";
        private const string _MsgLeaveOneDay = "<span class='req leave'>{0}</span> 1 day";
        private const string _MsgGeneralLeaveHalfDay = "<span class='req leave'>{0}</span> 0.5 day";
        private const string _MsgLeaveDuringWorking = "<span class='req leave'>{0} ({1}-{2})</span> {3} minutes";
        private const string _MsgChangeShiftOneDay = "<span class='req changeshift'>{0} <span class='arr'>&rarr;</span> {1}</span> {2}";
        private const string _MsgChangeShift = "<span class='req changeshift'>{0}</span>";
        private const string _MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";
        private const string _MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2}";
        private const string _MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2}";
        private const string _MsgKeepInView = "<span class='req keepinview'>{0}</span> {1}";
        private const string _MsgKeepInViewHalfday = "<span class='req keepinview'>{0} ({1} - {2})</span> {3}";

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

        private const int _MaxMunites = 210;

        private const string _EmailTemplatePathForRequester = "~/app_data/emails/RequestApproval.cshtml";
        private const string _EmailTemplatePathForCheckInOut = "~/app_data/emails/CheckInOutApproval.cshtml";
        private const string _EmailTemplatePathForLateInEarlyOut = "~/app_data/emails/LateInEarlyOutApproval.cshtml";
        private const string _EmailTemplatePathForChangeShift = "~/app_data/emails/ChangeShiftApproval.cshtml";
        private const string _EmailTemplatePathForLeave = "~/app_data/emails/LeaveApproval.cshtml";
        private const string _EmailTemplatePathForKeepInView = "~/app_data/emails/KeepInViewApproval.cshtml";
        private const string _EmailTemplatePathForLeaveCancelation = "~/app_data/emails/LeaveCancelation.cshtml";
        private const string _EmailTemplatePathForChangeShiftCancelation = "~/app_data/emails/ChangeShiftCancelation.cshtml";
        private const string _EmailTemplatePathForLateInEarlyOutCancelation = "~/app_data/emails/LateInEarlyOutCancelation.cshtml";
        private const string _EmailTemplatePathForCheckInOutCancelation = "~/app_data/emails/CheckInOutCancelation.cshtml";
        private const string _EmailTemplatePathForKeepInViewCancelation = "~/app_data/emails/KeepInViewCancelation.cshtml";

        private const string _LeaveRequestEmailTpl = "~/app_data/emails/Leave.Request.cshtml";

        #endregion
        private IEmployeeRepository _employeeRepository;
        private IEmailService _emailService;
        private IRequestRepository _requestRepository;

        public RequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._requestRepository = new RequestRepository();
            this._emailService = new EmailService();
        }

        public RequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            IRequestRepository requestRepository)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._requestRepository = requestRepository;
        }

        public IEmployeeRepository EmployeeRepository
        {
            get
            {
                return this._employeeRepository;
            }
        }

        public IEmailService EmailService
        {
            get
            {
                return this._emailService;
            }
        }

        public IRequestRepository RequestRepository
        {
            get
            {
                return this._requestRepository;
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

            switch (request.RequestType.Id)
            {
                case (int)RequestType.Leave:
                    message = GetMessageForLeave(request);
                    msg.Append(message);
                    break;
                case (int)RequestType.ChangeShift:
                    message = GetMessageForChangeShift(request);
                    msg.Append(message);
                    break;
                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    msg.AppendFormat(_MsgCheckInOut, request.RequestedDate.ToShortTime(), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestType.LateIn:
                    msg.AppendFormat(_MsgLateIn, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate));
                    break;
                case (int)RequestType.EarlyOut:
                    msg.AppendFormat(_MsgEarlyOut, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate));
                    break;
                case (int)RequestType.KeepInView:
                    if ((double)request.TotalDays == 0.5)
                    {
                        msg.AppendFormat(
                            _MsgKeepInViewHalfday, 
                            request.FromDate.ToDateMonthYearFormatedDate(), 
                            request.FromTime.ToShortTime(), 
                            request.ToTime.ToShortTime(), 
                            request.FromShift.Name);
                    }
                    else
                    {
                        msg.AppendFormat(_MsgKeepInView, request.FromDate.ToDateMonthYearFormatedDate(), request.FromShift.Name);
                    }
                    
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
                        result = fromDate.ToMonthAndDayFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
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
        /// Function to process a request.
        /// </summary>
        /// <param name="criteria">ProcessRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Process(ProcessRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
                if (result != null && result.Code == 0)
                {
                    var getRequestByIdCriteria = new GetRequestByIdCriteria()
                    {
                        RequestId = criteria.RequestId
                    };
                    RequestEntity request = this.RequestRepository.GetRequestById(getRequestByIdCriteria);
                    var templateEmailPathForRequester = HttpContext.Current.Server.MapPath(_EmailTemplatePathForRequester);
                    var templateEmailPathForRelated = this.GetEmailTemplatePath(request.RequestType.Id);

                    var sendMailTask = new Task(() 
                        => this.SendMailProcess(criteria.Action, request, criteria.ProcessedBy, templateEmailPathForRequester, templateEmailPathForRelated));
                    sendMailTask.Start();
                }
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Resources.Messages.InvalidInputData
                };
            }

            return result;
        }

        /// <summary>
        /// Approve, disapprove or reject a request,
        /// notification email will be sent also.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Return boolean.</returns>
        public CommandResult ProcessOvertime(ProcessSingleOvertimeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Approve, disapprove or reject a request,
        /// notification email will be sent also.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Return boolean.</returns>
        public CommandResult ProcessOvertime(ProcessMultipleOvertimeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Function to delete a request.
        /// </summary>
        /// <param name="criteria">DeleteRequest Criteria.</param>
        /// <returns>Result of deleted.</returns>
        public CommandResult Delete(DeleteRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = new CommandResult();

            if (criteria.IsValid())
            {
                var getRequestByIdCriteria = new GetRequestByIdCriteria()
                {
                    RequestId = criteria.RequestId
                };
                RequestEntity request = this.RequestRepository.GetRequestById(getRequestByIdCriteria);
                if (request != null)
                {
                    var templateEmailPath = GetEmailTemplatePathForCancelRequest(request.RequestType.Id);
                    var taskSendMail = new Task(() => this.SendMailForCancelRequest(request, templateEmailPath));
                    taskSendMail.Start();

                    result = Repository.Delete(criteria);
                }
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Resources.Messages.InvalidInputData
                };
            }

            return result;
        }

        public RequestEntity GetRequestById(GetRequestByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            RequestEntity request = null;
            if (criteria.IsValid())
            {
                request = RequestRepository.GetRequestById(criteria);
            }

            return request;
        }

        public IList<RequestEntity> GetRequests(GetRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> entityList = null;
            if (criteria.IsValid())
            {
                entityList = RequestRepository.GetRequests(criteria);
            }

            return entityList;
        }

        public decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            decimal number = 0;
            if (criteria.IsValid())
            {
                number = RequestRepository.GetNumberRequestInMonth(criteria);
            }

            return number;
        }

        /// <summary>
        /// Function to get Change shift request history.
        /// </summary>
        /// <param name="criteria">GetHistoryRequest Criteria.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoryRequest(GetHistoryRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> histories = null;
            if (criteria.IsValid())
            {
                histories = RequestRepository.GetHistoryRequest(criteria);
            }

            return histories;
        }

        /// <summary>
        /// Function to get mail to and mail cc of request.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="action">Action setting send mail.</param>
        /// <returns>Email Model.</returns>
        public EmailModel GetMailToAndMailCC(
            long requestId, 
            int action)
        {
            var emailModel = new EmailModel();
            var criteria = new GetRequestRelatedPeopleCriteria() 
            {
                RequestId = requestId,
                Action = action
            };
            RequestRelatedPeopleModel relatedPeople = RequestRepository.GetRequestRelatedPeople(criteria);
            if (relatedPeople.EmailSetting.AllowSendApprover)
            {
                emailModel = GetMailApprover(relatedPeople);
            }

            if (relatedPeople.EmailSetting.AllowSendVerifier
                && relatedPeople.Verifier != null
                && !string.IsNullOrWhiteSpace(relatedPeople.Verifier.Email)
                && (string.IsNullOrWhiteSpace(emailModel.SendTo)
                || !relatedPeople.Verifier.Email.Equals(relatedPeople.Approver.Email, StringComparison.OrdinalIgnoreCase)))
            {
                emailModel = GetMailVerifier(relatedPeople, emailModel);
            }

            if (relatedPeople.EmailSetting.AllowSendHR)
            {
                if (relatedPeople.Verifier != null)
                {
                    relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Verifier.Email).ToList();
                }

                if (relatedPeople.Approver != null)
                {
                    relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Approver.Email).ToList();
                }

                emailModel = GetMailHR(relatedPeople, emailModel);
            }

            if (relatedPeople.EmailSetting.AllowSendUser)
            {
                emailModel = GetMailOwnerRequest(relatedPeople, emailModel);
            }

            if (relatedPeople.EmailSetting.AllowSendMailGroup)
            {
                emailModel = GetMailGroup(relatedPeople, emailModel);
            }

            emailModel.IsAllowSendRequester = relatedPeople.EmailSetting.AllowSendUser;
            return emailModel;
        }

        /// <summary>
        /// Function to get string date for leave request.
        /// </summary>
        /// <param name="requestedDate">The requested date.</param>
        /// <param name="fromDate">The From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <param name="totalDays">Total days.</param>
        /// <returns>String detail date.</returns>
        protected string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays)
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
                    forDate = string.Format(_MailContentLeaveHalfDay, requestedDate.ToDateMonthYearFormatedDate(), fromDate.ToShortTime(), toDate.ToShortTime());
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
                _MsgLeaveHalfDay, GetMessageDateString(request.FromDate, request.ToDate), request.FromTime.ToShortTime(), request.ToTime.ToShortTime());
            }
            else
            {
                messageDetail = string.Format(
                    _MsgLeaveDuringWorking, leaveDate, request.FromDate.ToShortTime(), request.ToDate.ToShortTime(), (request.ToDate - request.FromDate).TotalMinutes);
            }

            return messageDetail;
        }

        /// <summary>
        /// Function to get message for change shift request.
        /// </summary>
        /// <param name="request">Request Entity.</param>
        /// <returns>Return detail of change shift request.</returns>
        private static string GetMessageForChangeShift(RequestEntity request)
        {
            var messageDetail = string.Empty;
            if (request.FromDate.Date == request.ToDate.Date)
            {
                messageDetail = string.Format(
                        _MsgChangeShiftOneDay,
                        request.FromShift.Name,
                        request.ToShift.Name,
                        request.FromDate.ToDateMonthYearFormatedDate());
            }
            else
            {
                var date = GetMessageDateString(request.FromDate, request.ToDate);
                messageDetail = string.Format(_MsgChangeShift, date);
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
            RequestEntity request,
            int processBy,
            string templateEmailPathForRequester,
            string templateEmailPathForRelated)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = request.Requester.Id };
            var getProcessUserByIdCriteria = new GetEmployeeByIdCriteria() { Empid = processBy };
            var employee = this._employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            var processUser = this._employeeRepository.GetEmployeeById(getProcessUserByIdCriteria);
            var action = GetActionProcessRequest(request, command);
            command = this.GetPassiveNameOfCommand(command);

            EmailModel emailModel = GetMailToAndMailCC(request.Id, action);
            if (emailModel.IsAllowSendRequester)
            {
                this.SendToRequester(command, request, employee, processUser, templateEmailPathForRequester);
            }

            if (!string.IsNullOrWhiteSpace(emailModel.SendTo))
            {
                this.SendToRelatedPeople(command, processUser, request, employee, emailModel.SendToNickName, emailModel.SendTo, emailModel.SendCc, templateEmailPathForRelated);
            }
        }

        /// <summary>
        /// Function ro get action setting send mail for process request.
        /// </summary>
        /// <param name="request">WorkingTime Request.</param>
        /// <param name="command">Process of request.</param>
        /// <returns>Action setting send mail.</returns>
        private int GetActionProcessRequest(RequestEntity request, string command)
        {
            var action = (int)SettingsSendMail.New;
            if (command == Command.Accept.ToString())
            {
                action = (int)SettingsSendMail.Verify;
            }

            if (command == Command.NotAccept.ToString())
            {
                action = (int)SettingsSendMail.NotVerify;
            }

            if (command == Command.Approve.ToString())
            {
                action = (int)SettingsSendMail.Approve;
            }

            if (command == Command.Reject.ToString())
            {
                action = (int)SettingsSendMail.Reject;
            }

            if (command == Command.Process.ToString())
            {
                action = (int)SettingsSendMail.Process;
            }

            if (command == Command.Void.ToString())
            {
                action = (int)SettingsSendMail.Void;
            }

            if (command == Command.Reset.ToString())
            {
                if (request.Status.Id == (byte)CurrentRequestStatus.Verified || request.Status.Id == (byte)CurrentRequestStatus.NotVerified)
                {
                    action = (int)SettingsSendMail.Reset1;
                }
                else if (request.Status.Id == (byte)CurrentRequestStatus.Approved || request.Status.Id == (byte)CurrentRequestStatus.Rejected)
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
        private void SendToRequester(string command, RequestEntity request, Employee employee, Employee processUser, string templateEmailPath)
        {
            string forDate = string.Empty;
            if (request.TotalDays > 1)
            {
                forDate = " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " + request.ToDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                forDate = " on " + request.FromDate.ToDateMonthYearFormatedDate();
            }

            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(body, employee.NickName, request.RequestType.Name, forDate, command, processUser.NickName, processUser.FullName);

            var subject = string.Format(
                Resources.Messages.ProcessSubject,
                request.RequestType.Name,
                forDate,
                command,
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
                case (short)RequestType.Leave:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLeaveCancelation);
                    break;
                case (short)RequestType.ChangeShift:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForChangeShiftCancelation);
                    break;
                case (short)RequestType.LateIn:
                case (short)RequestType.EarlyOut:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLateInEarlyOutCancelation);
                    break;
                case (short)RequestType.CheckIn:
                case (short)RequestType.CheckOut:
                    templateEmailPath = HttpContext.Current.Server.MapPath(_EmailTemplatePathForCheckInOutCancelation);
                    break;
                case (short)RequestType.KeepInView:
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
        private void SendMailForCancelRequest(RequestEntity request, string templateEmailPath)
        {
            var actionSendMail = (request.RequestType.Id == (short)RequestType.Leave && request.ReworkCount > 0)
                ? (int)SettingsSendMail.Delete
                : (int)SettingsSendMail.Cancel;

            EmailModel email = GetMailToAndMailCC(request.Id, actionSendMail);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                switch (request.RequestType.Id)
                {
                    case (short)RequestType.Leave:
                        this.SendMailForCancelLeaveRequest(request, email.SendTo, email.SendCc, email.SendToNickName, templateEmailPath);
                        break;
                    case (short)RequestType.ChangeShift:
                        this.SendMailForCancelChangeShiftRequest(request, email.SendTo, email.SendCc, email.SendToNickName, templateEmailPath);
                        break;
                    case (short)RequestType.LateIn:
                    case (short)RequestType.EarlyOut:
                        this.SendMailForCancelLateInEarlyOutRequest(request, email.SendTo, email.SendCc, email.SendToNickName, templateEmailPath);
                        break;
                    case (short)RequestType.CheckIn:
                    case (short)RequestType.CheckOut:
                        this.SendMailForCancelCheckInOutRequest(request, email.SendTo, email.SendCc, email.SendToNickName, templateEmailPath);
                        break;
                    case (short)RequestType.KeepInView:
                        this.SendMailForCancelKeepInViewRequest(request, email.SendTo, email.SendCc, email.SendToNickName, templateEmailPath);
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
            RequestEntity request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageLeaveRequestCanceled, request.Requester.NickName);
            var forDate = request.TotalDays == (decimal)0.5
                ? GetStringDateForLeaveRequest(request.FromDate, request.FromTime, request.ToTime, request.TotalDays)
                : GetStringDateForLeaveRequest(request.FromDate, request.FromDate, request.ToDate, request.TotalDays);

            var displayName = string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(body, sendToNickName, displayName, forDate, request.LeaveType.Name, request.Remark);
            
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
            RequestEntity request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageChangeShiftRequest, request.Requester.NickName);
            var forDate = GetMessageDateString(request.FromDate, request.ToDate);
            var displayName = string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(body, sendToNickName, displayName, forDate, request.Remark);

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
            RequestEntity request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageLateInEarlyOutRequest, request.RequestType.Name, request.Requester.NickName);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body, 
                sendToNickName,
                string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName),
                request.RequestType.Name, 
                request.FromDate.ToDateMonthYearFormatedDate(), 
                request.RequestType.Id == (short)RequestType.LateIn ? ("+" + request.TotalMins) : ("-" + request.TotalMins),
                request.Remark);

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
            RequestEntity request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageCheckInOutRequest, request.RequestType.Name, request.Requester.NickName);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                sendToNickName,
                string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName),
                request.RequestType.Name.ToLower(), 
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromShift.Name,
                request.RequestedDate.ToShortTime(),
                request.Remark);

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
            RequestEntity request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageKeepInViewRequest, request.Requester.NickName);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                sendToNickName,
                string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName),
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromShift.Name,
                request.Remark);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        /// <summary>
        /// Gets the passive name of command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The string of the passive name.</returns>
        private string GetPassiveNameOfCommand(string command)
        {
            if (command == Command.Accept.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Accepted.ToLower());
            }

            if (command == Command.NotAccept.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Accepted.ToLower());
            }

            if (command == Command.Approve.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Approved.ToLower());
            }

            if (command == Command.Reject.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Rejected.ToLower());
            }

            if (command == Command.Process.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Processed.ToLower());
            }

            if (command == Command.Void.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Void.ToLower());
            }

            if (command == Command.Reset.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Reseted.ToLower());
            }

            return string.Empty;
        }

        private EmailModel GetMailApprover(RequestRelatedPeopleModel relatedPeople)
        {
            var emailModel = new EmailModel();
            if (relatedPeople.Approver != null)
            {
                emailModel.SendTo = relatedPeople.Approver.Email;
                emailModel.SendToNickName = relatedPeople.Approver.NickName;
            }

            return emailModel;
        }

        /// <summary>
        /// Function to get mail of verifier.
        /// </summary>
        /// <param name="relatedPeople">Request Related People Model.</param>
        /// <param name="emailModel">In Email Model.</param>
        /// <returns>Out Email Model.</returns>
        private EmailModel GetMailVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Verifier != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Verifier.Email;
                    emailModel.SendToNickName = relatedPeople.Verifier.NickName;
                }
                else
                {
                    emailModel.SendTo += ";" + relatedPeople.Verifier.Email;
                }
            }

            return emailModel;
        }

        /// <summary>
        /// Function to get all emails of HR.
        /// </summary>
        /// <param name="relatedPeople">Request Related People Model.</param>
        /// <param name="emailModel">In Email Model.</param>
        /// <returns>Out Email Model.</returns>
        private EmailModel GetMailHR(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.ListHR != null && relatedPeople.ListHR.Any())
            {
                var mailHRs = string.Join(";", relatedPeople.ListHR.Select(x => x.Email != null ? x.Email : string.Empty));
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = mailHRs;
                    emailModel.SendToNickName = Labels.HR;
                }
                else
                {
                    emailModel.SendCc = mailHRs;
                }
            }

            return emailModel;
        }

        /// <summary>
        /// Function to get email of Owner request.
        /// </summary>
        /// <param name="relatedPeople">Request Related People Model.</param>
        /// <param name="emailModel">In Email Model.</param>
        /// <returns>Email Model.</returns>
        private EmailModel GetMailOwnerRequest(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Requester != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Requester.Email;
                    emailModel.SendToNickName = relatedPeople.Requester.NickName;
                }
                else if (string.IsNullOrWhiteSpace(emailModel.SendCc))
                {
                    emailModel.SendCc = relatedPeople.Requester.Email;
                }
                else
                {
                    emailModel.SendCc += ";" + relatedPeople.Requester.Email;
                }
            }

            return emailModel;
        }

        /// <summary>
        /// Function to get email group.
        /// </summary>
        /// <param name="relatedPeople">Request Related People Model.</param>
        /// <param name="emailModel">In Email Model.</param>
        /// <returns>Email Model.</returns>
        private EmailModel GetMailGroup(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Requester != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Requester.MailGroup;
                    emailModel.SendToNickName = Labels.All;
                }
                else if (string.IsNullOrWhiteSpace(emailModel.SendCc))
                {
                    emailModel.SendCc = relatedPeople.Requester.MailGroup;
                }
                else
                {
                    emailModel.SendCc += ";" + relatedPeople.Requester.MailGroup;
                }
            }

            return emailModel;
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
                case (short)RequestType.CheckIn:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForCheckInOut);
                    break;
                case (short)RequestType.CheckOut:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForCheckInOut);
                    break;
                case (short)RequestType.LateIn:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLateInEarlyOut);
                    break;
                case (short)RequestType.EarlyOut:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLateInEarlyOut);
                    break;
                case (short)RequestType.ChangeShift:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForChangeShift);
                    break;
                case (short)RequestType.Leave:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForLeave);
                    break;
                case (short)RequestType.KeepInView:
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
            RequestEntity request,
            Employee employee,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            switch (request.RequestType.Id)
            {
                case (short)RequestType.CheckIn:
                    var subjectCheckIn = string.Format(Messages.ApprovalNotificationCheckInSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, subjectCheckIn, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestType.CheckOut:
                    var subjectCheckOut = string.Format(Messages.ApprovalNotificationCheckOutSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, subjectCheckOut, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestType.LateIn:
                    var subjectLateIn = string.Format(Messages.ApprovalNotificationLateInSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, subjectLateIn, sendToNickName, mailTo, mailCc, true, templateEmailPath);
                    break;
                case (short)RequestType.EarlyOut:
                    var subjectEarlyOut = string.Format(Messages.ApprovalNotificationEarlyOutSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, subjectEarlyOut, sendToNickName, mailTo, mailCc, false, templateEmailPath);
                    break;
                case (short)RequestType.ChangeShift:
                    var subjectChangeShift = string.Format(Messages.ApprovalNotificationChangeShiftSubject, employee.NickName, command, processUser.NickName);
                    ChangeShiftRelatedMail(command, employee, processUser, request, subjectChangeShift, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestType.Leave:
                    var subjectLeave = string.Format(Messages.ApprovalNotificationLeaveSubject, employee.NickName, command, processUser.NickName);
                    LeaveRelatedMail(command, employee, processUser, request, subjectLeave, sendToNickName, mailTo, mailCc, templateEmailPath);
                    break;
                case (short)RequestType.KeepInView:
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
            RequestEntity request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            var criteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = request.FromDate
            };
            var shift = this.GetDataEntity<Shift>(criteria);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                sendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                shift != null ? shift.Name : string.Empty,
                request.FromDate.ToShortTime(),
                request.Remark);

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
            RequestEntity request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            bool isLateIn,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                sendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                isLateIn ? ("+" + request.TotalMins) : ("-" + request.TotalMins),
                request.Remark);
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
            RequestEntity request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = GetMessageDateString(request.FromDate, request.ToDate);
            body = string.Format(
                body,
                sendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                forDate,
                request.Remark);

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
            RequestEntity request,
            string subject,
            string sendToNickName,
            string mailTo,
            string sendCc,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = request.TotalDays == (decimal)0.5
                ? GetStringDateForLeaveRequest(request.FromDate, request.FromTime, request.ToTime, request.TotalDays)
                : GetStringDateForLeaveRequest(request.FromDate, request.FromDate, request.ToDate, request.TotalDays);
            body = string.Format(
                body, 
                sendToNickName, 
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command, 
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name, 
                forDate, 
                request.LeaveType.Name, 
                request.Remark);

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
            RequestEntity request,
            string subject,
            string sendToNickName,
            string mailTo,
            string mailCc,
            string templateEmailPath)
        {
            var criteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = employee.Id,
                Date = request.FromDate
            };
            var shift = this.GetDataEntity<Shift>(criteria);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                sendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                shift != null ? shift.Name : string.Empty,
                request.Remark);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                mailCc,
                subject,
                body);
        }
    }
}