namespace Fanex.NexHR.Services
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Hangfire;

    public class RequestMailService : BaseService, IRequestMailService
    {
        #region constant

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

        public RequestMailService()
        {
            employeeRepository = new EmployeeRepository();
            requestRepository = new RequestRepository();
            emailService = new EmailService();
        }

        public RequestMailService(
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

        public IRequestRepository RequestRepository
        {
            get
            {
                return this.requestRepository;
            }
        }

        public IEmailService EmailService
        {
            get
            {
                return emailService;
            }
        }

        public void SendMailForProcessSuccessfully(long requestId, int processedBy, string action)
        {
            var getRequestByIdCriteria = new GetRequestByIdCriteria
            {
                RequestId = requestId
            };

            RequestEntity request = requestRepository.GetRequestById(getRequestByIdCriteria);
            var templateEmailPathForRelated = GetEmailTemplatePath(request.RequestType.Id);

            BackgroundJob.Enqueue(() => SendMailProcess(action, request, processedBy, templateEmailPathForRelated));
        }

        public void SendMailForDeleteSuccessfully(long requestId)
        {
            var getRequestByIdCriteria = new GetRequestByIdCriteria
            {
                RequestId = requestId
            };
            RequestEntity request = requestRepository.GetRequestById(getRequestByIdCriteria);
            if (request != null)
            {
                var templateEmailPath = GetEmailTemplatePathForCancelRequest(request.RequestType.Id);
                BackgroundJob.Enqueue(() => SendMailForCancelRequest(request, templateEmailPath));
            }
        }

        public EmailModel GetMailToAndMailCC(long requestId, int action)
        {
            var emailModel = new EmailModel();
            var criteria = new GetRequestRelatedPeopleCriteria
            {
                RequestId = requestId,
                Action = action
            };
            RequestRelatedPeopleModel relatedPeople = requestRepository.GetRequestRelatedPeople(criteria);
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

        public void SendMailProcess(string command, RequestEntity request, int processBy, string templateEmailPathForRelated)
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

        public void SendMailForCancelRequest(RequestEntity request, string templateEmailPath)
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], mailTo, sendCc, subject, body);
        }

        private void SendMailForCancelChangeShiftRequest(
            RequestEntity request,
            string mailTo,
            string sendCc,
            string sendToNickName,
            string templateEmailPath)
        {
            var subject = string.Format(Messages.MessageChangeShiftRequest, request.Requester.NickName);
            var forDate = RequestService.GetMessageDateString(request.FromDate, request.ToDate);
            var displayName = string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName);
            var body = Utilities.ReadTextFile(templateEmailPath);
            body = string.Format(body, sendToNickName, displayName, forDate, request.Remark);

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], mailTo, sendCc, subject, body);
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], mailTo, sendCc, subject, body);
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], mailTo, sendCc, subject, body);
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], mailTo, sendCc, subject, body);
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
                    emailModel.Subject = string.Format(Messages.ApprovalNotificationCheckInSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.CheckOut:
                    emailModel.Subject = string.Format(Messages.ApprovalNotificationCheckOutSubject, employee.NickName, command, processUser.NickName);
                    CheckInOutRelatedMail(command, employee, processUser, request, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.LateIn:
                    emailModel.Subject = string.Format(Messages.ApprovalNotificationLateInSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, emailModel, true, templateEmailPath);
                    break;

                case (short)RequestType.EarlyOut:
                    emailModel.Subject = string.Format(Messages.ApprovalNotificationEarlyOutSubject, employee.NickName, command, processUser.NickName);
                    LateInEarlyOutRelatedMail(command, employee, processUser, request, emailModel, false, templateEmailPath);
                    break;

                case (short)RequestType.ChangeShift:
                    emailModel.Subject = string.Format(Messages.ApprovalNotificationChangeShiftSubject, employee.NickName, command, processUser.NickName);
                    ChangeShiftRelatedMail(command, employee, processUser, request, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.Leave:
                    emailModel.Subject = string.Format(Messages.ApprovalNotificationLeaveSubject, employee.NickName, command, processUser.NickName);
                    LeaveRelatedMail(command, employee, processUser, request, emailModel, templateEmailPath);
                    break;

                case (short)RequestType.KeepInView:
                    emailModel.Subject = string.Format(Messages.ApprovalNotificationKeepInViewSubject, employee.NickName, command, processUser.NickName);
                    KeepInViewRelatedMail(command, employee, processUser, request, emailModel, templateEmailPath);
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], emailModel.SendTo, emailModel.SendCC, emailModel.Subject, body);
        }

        private void LateInEarlyOutRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], emailModel.SendTo, emailModel.SendCC, emailModel.Subject, body);
        }

        private void ChangeShiftRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
            EmailModel emailModel,
            string templateEmailPath)
        {
            var body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = RequestService.GetMessageDateString(request.FromDate, request.ToDate);
            body = string.Format(
                body,
                emailModel.SendToNickName,
                string.Format("{0} ({1})", employee.NickName, employee.FullName),
                command,
                string.Format("{0} ({1})", processUser.NickName, processUser.FullName),
                request.RequestType.Name,
                forDate,
                request.Remark);

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], emailModel.SendTo, emailModel.SendCC, emailModel.Subject, body);
        }

        private void LeaveRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], emailModel.SendTo, emailModel.SendCC, emailModel.Subject, body);
        }

        private void KeepInViewRelatedMail(
            string command,
            Employee employee,
            Employee processUser,
            RequestEntity request,
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

            emailService.SendEmail(ConfigurationManager.AppSettings["Email"], emailModel.SendTo, emailModel.SendCC, emailModel.Subject, body);
        }
    }
}