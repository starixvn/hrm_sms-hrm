namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Hangfire;

    public class CheckInOutRequestService : ICheckInOutRequestService
    {
        private const string CheckInOutTmp = "~/app_data/emails/CheckInOut.Request.cshtml";
        private const string EmailTemplatePathForCheckInOutCancelation = "~/app_data/emails/CheckInOutCancelation.cshtml";
        private readonly IEmployeeRepository employeeRepository;
        private readonly ICheckInOutRequestRepository checkInOutRequestRepository;
        private readonly IBaseService baseService;
        private readonly IRequestMailService requestMailService;
        private readonly IEmailService emailService;

        public CheckInOutRequestService(
            IEmployeeRepository employeeRepository,
            ICheckInOutRequestRepository checkInOutRequestRepository,
            IBaseService baseService,
            IRequestMailService requestMailService,
            IEmailService emailService)
        {
            this.employeeRepository = employeeRepository;
            this.checkInOutRequestRepository = checkInOutRequestRepository;
            this.baseService = baseService;
            this.requestMailService = requestMailService;
            this.emailService = emailService;
        }

        public IList<CheckInOutRequest> GetByDate(GetCheckInOutRequestByDateCriteria criteria)
        {
            IList<CheckInOutRequest> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = checkInOutRequestRepository.GetByDate(criteria);
            }

            return requests ?? new List<CheckInOutRequest>();
        }

        public CheckInOutRequest GetById(long requestId)
        {
            var criteria = new GetCheckInOutRequestByIdCriteria
            {
                RequestId = requestId
            };
            return checkInOutRequestRepository.GetById(criteria);
        }

        public IList<CheckInOutRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetCheckInOutRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return checkInOutRequestRepository.GetHistoryById(criteria);
        }

        public CommandResult Delete(long requestId)
        {
            var criteria = new DeleteCheckInOutRequestCriteria
            {
                RequestId = requestId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
            BackgroundJob.Enqueue(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
            BackgroundJob.Enqueue(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
        }

        public void SendMailForInsertSuccessfully(InsertCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.CheckInOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.RequestTime.ToShortTime(),
                    criteria.Remark,
                ConfigSettings.ApplicationDomain);

            emailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);
        }

        public void SendMailForUpdateSuccessfully(UpdateCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria
            {
                EmpId = criteria.EmpId
            };
            Employee employee = employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);

            email.Subject = string.Format(
                "{0} {1} on {2}",
                Messages.UpdatedCheckInOutEmailSubject,
                employee.NickName,
                criteria.OnDate.ToDateMonthYearFormatedDate());

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.RequestTime.ToShortTime(),
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
            var request = GetById(requestId);
            if (request != null)
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCheckInOutCancelation);
                BackgroundJob.Enqueue(() => SendMailForDeleteRequest(request, templateEmailPath));
            }
        }

        public void SendMailForDeleteRequest(CheckInOutRequest request, string templateEmailPath)
        {
            EmailModel email = requestMailService.GetMailToAndMailCC(request.RequestId, (int)SettingsSendMail.Cancel);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                var subject = string.Format(Messages.MessageCheckInOutRequest, request.RequestTypeName, request.Employee.NickName);
                var body = Utilities.ReadTextFile(templateEmailPath);
                body = string.Format(
                body,
                email.SendToNickName,
                string.Format("{0} ({1})", request.Employee.NickName, request.Employee.FullName),
                request.RequestTypeName.ToLowerInvariant(),
                request.OnDate.ToDateMonthYearFormatedDate(),
                request.ShiftName,
                request.RequestTime.ToShortTime(),
                request.Remark);

                emailService.SendEmail(email.Sender, email.SendTo, email.SendCC, subject, body);
            }
        }
    }
}