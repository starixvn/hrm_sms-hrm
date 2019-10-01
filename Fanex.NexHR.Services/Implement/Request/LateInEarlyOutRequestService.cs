namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Hangfire;

    public class LateInEarlyOutRequestService : ILateInEarlyOutRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/LateInEarlyOut.Request.cshtml";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/LateInEarlyOut.Cancel.cshtml";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/LateInEarlyOut.Approval.cshtml";
        private readonly IEmployeeRepository employeeRepository;
        private readonly ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;
        private readonly IBaseService baseService;

        public LateInEarlyOutRequestService(
            IEmployeeRepository employeeRepository,
            ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository,
            IRequestMailService requestMailService,
            IEmailService emailService,
            IBaseService baseService)
        {
            this.employeeRepository = employeeRepository;
            this.lateInEarlyOutRequestRepository = lateInEarlyOutRequestRepository;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
            this.baseService = baseService;
        }

        public IList<LateInEarlyOutRequest> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria)
        {
            IList<LateInEarlyOutRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = lateInEarlyOutRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<LateInEarlyOutRequest>();
        }

        public LateInEarlyOutRequest GetById(long requestId)
        {
            var criteria = new GetLateInEarlyOutRequestByIdCriteria
            {
                RequestId = requestId
            };
            return lateInEarlyOutRequestRepository.GetById(criteria);
        }

        public IList<LateInEarlyOutRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetLateInEarlyOutRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return lateInEarlyOutRequestRepository.GetHistoryById(criteria);
        }

        public EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria)
        {
            return baseService.GetDataEntity<EmployeeLateInEarlyOutModel>(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteLateInEarlyOutRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult ApproveRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new ApproveLateInEarlyOutRequestCriteria
            {
                RequestId = param.RequestId,
                RoleId = param.RoleId,
                Comment = param.Comment,
                ActorId = actorId,
                DisciplineXml = param.Disciplines.Any() ? param.Disciplines.ToXML().ToString() : string.Empty
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult ResetRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new ResetLateInEarlyOutRequestCriteria
            {
                RequestId = param.RequestId,
                RoleId = param.RoleId,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult RejectRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new RejectLateInEarlyOutRequestCriteria
            {
                RequestId = param.RequestId,
                RoleId = param.RoleId,
                Comment = param.Comment,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult VoidRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new VoidLateInEarlyOutRequestCriteria
            {
                RequestId = param.RequestId,
                RoleId = param.RoleId,
                Comment = param.Comment,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.LateInEarlyOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                criteria.Remark,
                ConfigSettings.ApplicationDomain);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.UpdatedLateInEarlyOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.LateIn ? "a " + Labels.LateIn : "an " + Labels.EarlyOut,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                criteria.Remark,
                ConfigSettings.ApplicationDomain);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForDeleteSuccessfully(long requestId)
        {
            LateInEarlyOutRequest request = GetById(requestId);
            if (request != null)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                BackgroundJob.Enqueue(() => SendMailForDeleteRequest(request, templateEmailPath));
            }
        }

        public void SendMailForDeleteRequest(LateInEarlyOutRequest request, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, (byte)SettingsSendMail.Cancel);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                var subject = string.Format(Messages.MessageLateInEarlyOutRequest, request.RequestTypeName, request.Employee.NickName);
                var body = Utilities.ReadTextFile(templateEmailPath);
                body = string.Format(
                    body,
                   email.SendToNickName,
                    string.Format("{0} ({1})", request.Employee.NickName, request.Employee.FullName),
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (short)RequestType.LateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
                    request.Remark);

                emailService.SendEmail(email.Sender, email.SendTo, email.SendCC, subject, body);
            }
        }

        public void SendMailAfterProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
        {
            LateInEarlyOutRequest request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                byte action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                string commandMessage = requestMailService.GetCommandMessage(command);
                EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, action);

                email.Subject = request.RequestTypeId == (byte)RequestType.LateIn
                    ? string.Format(Messages.ApprovalNotificationLateInSubject, request.Employee.NickName, commandMessage, actor.Nickname)
                    : string.Format(Messages.ApprovalNotificationEarlyOutSubject, request.Employee.NickName, commandMessage, actor.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Employee.DisplayName,
                    commandMessage,
                    actor.DisplayName,
                    request.RequestTypeName,
                    request.OnDate.ToDateMonthYearFormatedDate(),
                    request.RequestTypeId == (byte)RequestType.LateIn ? ("+" + request.LateInMins) : ("-" + request.EarlyOutMins),
                    request.Remark);

                BackgroundJob.Enqueue(() => emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));
            }
        }
    }
}