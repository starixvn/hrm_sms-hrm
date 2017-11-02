namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public class RequestService : BaseService, IRequestService
    {
        #region constant

        private const string _MsgLeave = "<span class='req leave'>{1}</span> {0} days";
        private const string _MsgLeaveHalfDay = "<span class='req leave'>{0} ({1}-{2})</span> 0.5 day";
        private const string _MsgLeaveOneDay = "<span class='req leave'>{0}</span> 1 day";
        private const string _MsgLeavePublicHoliday = "<span class='req leave'>{0}</span>";
        private const string _MsgChangeShiftOneDay = "<span class='req changeshift'>{0} <span class='arr'>&rarr;</span> {1}</span> {2}";
        private const string _MsgChangeShift = "<span class='req changeshift'>{0}</span>";
        private const string _MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";
        private const string _MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2} ({3})";
        private const string _MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2} ({3})";
        private const string _MsgKeepInView = "<span class='req keepinview'>{0}</span> {1}";
        private const string _MsgKeepInViewHalfday = "<span class='req keepinview'>{0} ({1} - {2})</span> {3}";

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

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

        #endregion constant

        private readonly IEmployeeRepository employeeRepository;
        private readonly IEmailService emailService;
        private readonly IRequestRepository requestRepository;

        public RequestService()
        {
            employeeRepository = new EmployeeRepository();
            requestRepository = new RequestRepository();
            emailService = new EmailService();
        }

        public RequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            IRequestRepository requestRepository)
        {
            this.employeeRepository = employeeRepository;
            this.emailService = emailService;
            this.requestRepository = requestRepository;
        }

        public IEmployeeRepository EmployeeRepository
        {
            get
            {
                return this.employeeRepository;
            }
        }

        public IEmailService EmailService
        {
            get
            {
                return this.emailService;
            }
        }

        public IRequestRepository RequestRepository
        {
            get
            {
                return this.requestRepository;
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        public static string ToRequestMessage(RequestEntity request)
        {
            var msg = new StringBuilder();
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
                    message = GetMessageForCheckInOut(request);
                    msg.Append(message);
                    break;

                case (int)RequestType.LateIn:
                    var actualLateIn = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("+" + request.ActualLateInEarlyOut + "'");
                    msg.AppendFormat(_MsgLateIn, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate), actualLateIn);
                    break;

                case (int)RequestType.EarlyOut:
                    var actualEarlyOut = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("-" + request.ActualLateInEarlyOut + "'");
                    msg.AppendFormat(_MsgEarlyOut, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate), actualEarlyOut);
                    break;

                case (int)RequestType.KeepInView:
                    double totalDays = (double)request.TotalDays;

                    message = GetMessageForKeepInView(request, totalDays);
                    msg.Append(message);

                    break;

                default:
                    msg.AppendFormat(string.Empty);
                    break;
            }

            return msg.ToString();
        }

        private static string GetMessageForCheckInOut(RequestEntity request)
        {
            var time = request.RequestedDate.Day == request.FromDate.Day ? request.RequestedDate.ToShortTime() : (request.RequestedDate.ToShortTime() + "+");
            return string.Format(_MsgCheckInOut, time, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
        }

        private static string GetMessageForKeepInView(RequestEntity request, double totalDays)
        {
            if (totalDays.Equals(0.5))
            {
                return string.Format(
                    _MsgKeepInViewHalfday,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime(),
                    request.FromShift.Name);
            }

            return string.Format(_MsgKeepInView, request.FromDate.ToDateMonthYearFormatedDate(), request.FromShift.Name);
        }

        public static string GetMessageDateString(DateTime fromDate, DateTime toDate)
        {
            if ((toDate - fromDate).Days < 0)
            {
                return fromDate.ToDateMonthYearFormatedDate();
            }

            if (fromDate.Year != toDate.Year)
            {
                return fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
            }

            string result = string.Empty;
            result = (fromDate.Month != toDate.Month) ?
                fromDate.ToMonthAndDayFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate() :
                fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();

            return result;
        }

        public void SendMailForProcessSuccessfully(long requestId, int processedBy, string action)
        {
            var getRequestByIdCriteria = new GetRequestByIdCriteria
            {
                RequestId = requestId
            };

            RequestEntity request = requestRepository.GetRequestById(getRequestByIdCriteria);
            var templateEmailPathForRelated = GetEmailTemplatePath(request.RequestType.Id);

            Task.Factory.StartNew(() =>
            {
                SendMailProcess(action, request, processedBy, templateEmailPathForRelated);
            });
        }

        public void SendMailForDeleteSuccessfully(long requestId)
        {
            var getRequestByIdCriteria = new GetRequestByIdCriteria
            {
                RequestId = requestId
            };
            RequestEntity request = RequestRepository.GetRequestById(getRequestByIdCriteria);
            if (request != null)
            {
                var templateEmailPath = GetEmailTemplatePathForCancelRequest(request.RequestType.Id);
                Task.Factory.StartNew(() =>
                {
                    SendMailForCancelRequest(request, templateEmailPath);
                });
            }
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

        public EmailModel GetMailToAndMailCC(
            long requestId,
            int action)
        {
            var emailModel = new EmailModel();
            var criteria = new GetRequestRelatedPeopleCriteria
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
                && !string.IsNullOrWhiteSpace(relatedPeople.Verifier.Email)
                && (string.IsNullOrWhiteSpace(emailModel.SendTo)
                || !relatedPeople.Verifier.Email.Equals(relatedPeople.Approver.Email, StringComparison.OrdinalIgnoreCase)))
            {
                emailModel = GetMailVerifier(relatedPeople, emailModel);
            }

            if (relatedPeople.EmailSetting.AllowSendHR)
            {
                RelatedPeopleAllowSendHR(relatedPeople);
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

        private static void RelatedPeopleAllowSendHR(RequestRelatedPeopleModel relatedPeople)
        {
            if (relatedPeople.Verifier != null)
            {
                relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Verifier.Email).ToList();
            }

            if (relatedPeople.Approver != null)
            {
                relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Approver.Email).ToList();
            }
        }

        public string CheckRoleAccordingToRequestType(RequestViewType requestViewType)
        {
            string role;
            switch (requestViewType)
            {
                case RequestViewType.PersonalSpace:
                    role = ((int)RoleSystem.Verifier).ToString() + "," + ((int)RoleSystem.Approver).ToString();
                    break;

                case RequestViewType.HRSpace:
                    role = ((int)RoleSystem.HR).ToString();
                    break;

                default:
                    role = ((int)RoleSystem.Member).ToString();
                    break;
            }

            return role;
        }

        public VerifierAndApproverForRequest GetVerifierAndApprover(GetVerifierAndApproverCriteria criteria)
        {
            VerifierAndApproverForRequest supervisor = new VerifierAndApproverForRequest();

            if (criteria != null && criteria.IsValid())
            {
                supervisor = RequestRepository.GetVerifierAndApprover(criteria);
            }

            return supervisor;
        }

        public RequestEntity GetCheckInOutRequestToIgnoreById(GetCheckInOutRequestToIgnoreByIdCriteria criteria)
        {
            RequestEntity request = null;
            if (criteria != null && criteria.IsValid())
            {
                request = RequestRepository.GetRequestToIgnoreById(criteria);
            }

            return request;
        }

        public RequestEntity GetlateInEarlyOutRequestToIgnoreById(GetLateInEarlyOutRequestToIgnoreByIdCriteria criteria)
        {
            RequestEntity request = null;
            if (criteria != null && criteria.IsValid())
            {
                request = RequestRepository.GetRequestToIgnoreById(criteria);
            }

            return request;
        }

        public IList<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(GetChangeShiftDetailToIgnoreByIdCriteria criteria)
        {
            List<ChangeShiftDetailModel> details = GetData<ChangeShiftDetailModel>(criteria).ToList();
            return details;
        }

        public IList<LeaveDetailModel> GetLeaveRequestDetailToIgnoreById(GetLeaveRequestDetailToIgnoreByIdCriteria criteria)
        {
            IList<LeaveDetailModel> leaveDetails = GetData<LeaveDetailModel>(criteria).ToList();
            return leaveDetails;
        }

        protected static string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            if (totalDays > 1)
            {
                return string.Format(_MailContentLeave, fromDate.ToDateMonthYearFormatedDate(), toDate.ToDateMonthYearFormatedDate(), totalDays);
            }

            if (totalDays == 1)
            {
                return string.Format(_MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
            }

            return string.Format(_MailContentLeaveHalfDay, requestedDate.ToDateMonthYearFormatedDate(), fromDate.ToShortTime(), toDate.ToShortTime());
        }

        private static string GetMessageForLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;

            messageDetail = (request.LeaveType != null && request.LeaveType.Id == (int)LeaveTypes.GeneralLeave) ?
                            GetMessageForGeneralLeave(request) : GetMessageForOtherLeave(request);

            return messageDetail;
        }

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

        private static string GetMessageForGeneralLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;
            var leaveDate = GetMessageDateString(request.FromDate, request.ToDate);

            if (request.TotalDays >= 1)
            {
                messageDetail = string.Format("<span class='req leave'>{0}</span> {1}", leaveDate, request.LeaveCountDetails);
            }

            if ((request.TotalDays - (decimal)0.5) == 0)
            {
                messageDetail = string.Format(
                "<span class='req leave'>{0} ({1}-{2})</span> {3}",
                leaveDate,
                request.FromTime.ToShortTime(),
                request.ToTime.ToShortTime(),
                request.LeaveCountDetails);
            }

            return messageDetail;
        }

        private static string GetMessageForOtherLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;
            var leaveDate = GetMessageDateString(request.FromDate, request.ToDate);

            if (request.TotalDays > 1)
            {
                messageDetail = string.Format(_MsgLeave, request.TotalDays, leaveDate);
            }
            else if (request.TotalDays == 1)
            {
                messageDetail = string.Format(_MsgLeaveOneDay, leaveDate);
            }
            else if ((request.TotalDays - (decimal)0.5) == 0)
            {
                messageDetail = string.Format(
                _MsgLeaveHalfDay, leaveDate, request.FromTime.ToShortTime(), request.ToTime.ToShortTime());
            }
            else
            {
                messageDetail = string.Format(_MsgLeavePublicHoliday, leaveDate);
            }

            return messageDetail;
        }

        private void SendMailProcess(
            string command,
            RequestEntity request,
            int processBy,
            string templateEmailPathForRelated)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria { EmpId = request.Requester.Id };
            var getProcessUserByIdCriteria = new GetEmployeeByIdCriteria { EmpId = processBy };
            var employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            var processUser = employeeRepository.GetEmployeeById(getProcessUserByIdCriteria);
            var action = GetActionProcessRequest(request, command);

            string commandGetPassiveName = GetPassiveNameOfCommand(command);

            EmailModel emailModel = GetMailToAndMailCC(request.Id, action);

            if (!string.IsNullOrWhiteSpace(emailModel.SendTo))
            {
                SendToRelatedPeople(commandGetPassiveName, processUser, request, employee, emailModel, templateEmailPathForRelated);
            }
        }

        private static int GetActionProcessRequest(RequestEntity request, string command)
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
                action = GetActionResetRequest(request.Status.Id);
            }

            return action;
        }

        private static int GetActionResetRequest(byte statusId)
        {
            byte action = 0;
            switch (statusId)
            {
                case (byte)CurrentRequestStatus.Verified:
                case (byte)CurrentRequestStatus.NotVerified:
                    action = (int)SettingsSendMail.Reset1;
                    break;

                case (byte)CurrentRequestStatus.Approved:
                case (byte)CurrentRequestStatus.Rejected:
                    action = (int)SettingsSendMail.Reset2;
                    break;

                default:
                    action = (int)SettingsSendMail.Reset3;
                    break;
            }

            return action;
        }

        private static string GetEmailTemplatePathForCancelRequest(short requestType)
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

        private void SendMailForCancelRequest(RequestEntity request, string templateEmailPath)
        {
            var actionSendMail = (request.RequestType.Id == (short)RequestType.Leave && request.ReworkCount > 0)
                ? (int)SettingsSendMail.Delete
                : (int)SettingsSendMail.Cancel;

            EmailModel email = GetMailToAndMailCC(request.Id, actionSendMail);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                HandleSendMailCancelRequest(request, templateEmailPath, email);
            }
        }

        private void HandleSendMailCancelRequest(RequestEntity request, string templateEmailPath, EmailModel email)
        {
            switch (request.RequestType.Id)
            {
                case (short)RequestType.Leave:
                    SendMailForCancelLeaveRequest(request, email.SendTo, email.SendCC, email.SendToNickName, templateEmailPath);
                    break;

                case (short)RequestType.ChangeShift:
                    SendMailForCancelChangeShiftRequest(request, email.SendTo, email.SendCC, email.SendToNickName, templateEmailPath);
                    break;

                case (short)RequestType.LateIn:
                case (short)RequestType.EarlyOut:
                    SendMailForCancelLateInEarlyOutRequest(request, email.SendTo, email.SendCC, email.SendToNickName, templateEmailPath);
                    break;

                case (short)RequestType.CheckIn:
                case (short)RequestType.CheckOut:
                    SendMailForCancelCheckInOutRequest(request, email.SendTo, email.SendCC, email.SendToNickName, templateEmailPath);
                    break;

                case (short)RequestType.KeepInView:
                    SendMailForCancelKeepInViewRequest(request, email.SendTo, email.SendCC, email.SendToNickName, templateEmailPath);
                    break;

                default:
                    return;
            }
        }

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

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

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

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

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

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

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
                request.RequestType.Name.ToLowerInvariant(),
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromShift.Name,
                request.RequestedDate.ToShortTime(),
                request.Remark);

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

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

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                sendCc,
                subject,
                body);
        }

        private static string GetPassiveNameOfCommand(string command)
        {
            if (command == Command.Accept.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Accepted.ToLowerInvariant());
            }

            if (command == Command.NotAccept.ToString())
            {
                return string.Format(Messages.HasNotBeen, Labels.Accepted.ToLowerInvariant());
            }

            if (command == Command.Approve.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Approved.ToLowerInvariant());
            }

            if (command == Command.Reject.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Rejected.ToLowerInvariant());
            }

            if (command == Command.Process.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Processed.ToLowerInvariant());
            }

            if (command == Command.Void.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant());
            }

            if (command == Command.Reset.ToString())
            {
                return string.Format(Messages.HasBeen, Labels.Reseted.ToLowerInvariant());
            }

            return string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant());
        }

        private static EmailModel GetMailApprover(RequestRelatedPeopleModel relatedPeople)
        {
            var emailModel = new EmailModel();
            if (relatedPeople.Approver != null)
            {
                emailModel.SendTo = relatedPeople.Approver.Email;
                emailModel.SendToNickName = relatedPeople.Approver.NickName;
            }

            return emailModel;
        }

        private static EmailModel GetMailVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
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

        private static EmailModel GetMailHR(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.ListHR != null && relatedPeople.ListHR.Any())
            {
                var mailHRs = string.Join(";", relatedPeople.ListHR.Select(x => x.Email ?? string.Empty));
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = mailHRs;
                    emailModel.SendToNickName = Labels.HR;
                }
                else
                {
                    emailModel.SendCC = mailHRs;
                }
            }

            return emailModel;
        }

        private static EmailModel GetMailOwnerRequest(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Requester != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Requester.Email;
                    emailModel.SendToNickName = relatedPeople.Requester.NickName;
                }
                else
                {
                    emailModel.SendCC = AddMailToList(emailModel.SendCC, relatedPeople.Requester.Email);
                }
            }

            return emailModel;
        }

        private static string AddMailToList(string mailList, string mail)
        {
            string fullMailList = string.Empty;

            if (string.IsNullOrWhiteSpace(mail))
            {
                return mailList;
            }

            if (string.IsNullOrWhiteSpace(mailList))
            {
                return mail;
            }

            fullMailList = mailList + ";" + mail;

            return fullMailList;
        }

        private static EmailModel GetMailGroup(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.Requester != null)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Requester.MailGroup;
                    emailModel.SendToNickName = Labels.All;
                }
                else if (string.IsNullOrWhiteSpace(emailModel.SendCC))
                {
                    emailModel.SendCC = relatedPeople.Requester.MailGroup;
                }
                else
                {
                    emailModel.SendCC += ";" + relatedPeople.Requester.MailGroup;
                }
            }

            return emailModel;
        }

        private static string GetEmailTemplatePath(short requestType)
        {
            string path;
            switch (requestType)
            {
                case (short)RequestType.CheckIn:
                case (short)RequestType.CheckOut:
                    path = HttpContext.Current.Server.MapPath(_EmailTemplatePathForCheckInOut);
                    break;

                case (short)RequestType.LateIn:
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

                default:
                    path = string.Empty;
                    break;
            }

            return path;
        }

        private void SendToRelatedPeople(
            string command,
            Employee processUser,
            RequestEntity request,
            Employee employee,
            EmailModel emailModel,
            string templateEmailPath)
        {
            switch (request.RequestType.Id)
            {
                case (short)RequestType.CheckIn:
                    var subjectCheckIn = string.Format(Messages.ApprovalNotificationCheckInSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, subjectCheckIn, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.CheckOut:
                    var subjectCheckOut = string.Format(Messages.ApprovalNotificationCheckOutSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, subjectCheckOut, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.LateIn:
                    var subjectLateIn = string.Format(Messages.ApprovalNotificationLateInSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, subjectLateIn, emailModel, true, templateEmailPath);
                    break;

                case (short)RequestType.EarlyOut:
                    var subjectEarlyOut = string.Format(Messages.ApprovalNotificationEarlyOutSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, subjectEarlyOut, emailModel, false, templateEmailPath);
                    break;

                case (short)RequestType.ChangeShift:
                    var subjectChangeShift = string.Format(Messages.ApprovalNotificationChangeShiftSubject, employee.NickName, command, processUser.NickName);
                    ChangeShiftRelatedMail(command, employee, processUser, request, subjectChangeShift, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.Leave:
                    var subjectLeave = string.Format(Messages.ApprovalNotificationLeaveSubject, employee.NickName, command, processUser.NickName);
                    LeaveRelatedMail(command, employee, processUser, request, subjectLeave, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.KeepInView:
                    var subjectKeepInView = string.Format(Messages.ApprovalNotificationKeepInViewSubject, employee.NickName, command, processUser.NickName);
                    KeepInViewRelatedMail(command, employee, processUser, request, subjectKeepInView, emailModel, templateEmailPath);
                    break;

                default:
                    return;
            }
        }

        private void CheckInOutRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
            string subject,
            EmailModel emailModel,
            string templateEmailPath)
        {
            var criteria = new GetEmployeeShiftByDateCriteria
            {
                EmployeeId = employee.Id,
                Date = request.FromDate
            };
            var shift = this.GetDataEntity<Shift>(criteria);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                emailModel.SendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                shift != null ? shift.Name : string.Empty,
                request.RequestedDate.ToShortTime(),
                request.Remark);

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                emailModel.SendTo,
                emailModel.SendCC,
                subject,
                body);
        }

        private void LateInEarlyOutRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
            string subject,
            EmailModel emailModel,
            bool isLateIn,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                emailModel.SendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                isLateIn ? ("+" + request.TotalMins) : ("-" + request.TotalMins),
                request.Remark);
            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                emailModel.SendTo,
                emailModel.SendCC,
                subject,
                body);
        }

        private void ChangeShiftRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
            string subject,
            EmailModel emailModel,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = GetMessageDateString(request.FromDate, request.ToDate);
            body = string.Format(
                body,
                emailModel.SendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                forDate,
                request.Remark);

            emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                emailModel.SendTo,
                emailModel.SendCC,
                subject,
                body);
        }

        private void LeaveRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
            string subject,
            EmailModel emailModel,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = request.TotalDays == (decimal)0.5
                ? GetStringDateForLeaveRequest(request.FromDate, request.FromTime, request.ToTime, request.TotalDays)
                : GetStringDateForLeaveRequest(request.FromDate, request.FromDate, request.ToDate, request.TotalDays);
            body = string.Format(
                body,
                emailModel.SendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                forDate,
                request.LeaveType.Name,
                request.Remark);

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                emailModel.SendTo,
                emailModel.SendCC,
                subject,
                body);
        }

        private void KeepInViewRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
            string subject,
            EmailModel emailModel,
            string templateEmailPath)
        {
            var criteria = new GetEmployeeShiftByDateCriteria
            {
                EmployeeId = employee.Id,
                Date = request.FromDate
            };
            var shift = this.GetDataEntity<Shift>(criteria);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(
                body,
                emailModel.SendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                shift != null ? shift.Name : string.Empty,
                request.Remark);

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                emailModel.SendTo,
                emailModel.SendCC,
                subject,
                body);
        }
    }
}