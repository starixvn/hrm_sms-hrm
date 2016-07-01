namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System.Threading.Tasks;
    using System.Web;

    public class LateInEarlyOutRequestService : RequestService, ILateInEarlyOutRequestService
    {
        private const string LateInEarlyOutEmailTpl = "~/app_data/emails/LateInEarlyOut.Request.cshtml";

        private readonly ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository;

        public LateInEarlyOutRequestService()
            : this(null)
        {
        }

        public LateInEarlyOutRequestService(ILateInEarlyOutRequestRepository lateInEarlyOutRequestRepository)
        {
            this.lateInEarlyOutRequestRepository = lateInEarlyOutRequestRepository ?? new LateInEarlyOutRequestRepository();
        }

        public EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria)
        {
            EmployeeLateInEarlyOutModel lateInEarlyOut = GetDataEntity<EmployeeLateInEarlyOutModel>(criteria);
            return lateInEarlyOut;
        }

        public void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(LateInEarlyOutEmailTpl);
            var taskSendMail = new Task(() => SendMailForInsertSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        public void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria)
        {
            var templateEmailPath = HttpContext.Current.Server.MapPath(LateInEarlyOutEmailTpl);
            var taskSendMail = new Task(() => SendMailForUpdateSuccessfully(criteria, templateEmailPath));
            taskSendMail.Start();
        }

        private void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.LateInEarlyOutEmailSubject + " " + employee.NickName
                + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.LateIn ? "a " + Resources.Labels.LateIn : "an " + Resources.Labels.EarlyOut,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                criteria.Remark);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }

        private void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            Employee employee = EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.UpdatedLateInEarlyOutEmailSubject + " " + employee.NickName
                + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();

            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.LateIn ? "a " + Resources.Labels.LateIn : "an " + Resources.Labels.EarlyOut,
                criteria.OnDate.ToDateMonthYearFormatedDate(),
                criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                criteria.Remark);

            EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }
    }
}