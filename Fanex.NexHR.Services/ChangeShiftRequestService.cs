// -----------------------------------------------------------------------
// <copyright file="ChangeShiftRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels.Request;

    /// <summary>
    /// Change Shift Request Service.
    /// </summary>
    public class ChangeShiftRequestService : RequestService, IChangeShiftRequestService
    {
        private const string _ChangeShiftTmp = "~/app_data/emails/ChangeShift.Request.cshtml";
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private string _baseUrl;

        public ChangeShiftRequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._emailService = new EmailService();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public ChangeShiftRequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Function to insert change shift request.
        /// </summary>
        /// <param name="criteria">InsertLeaveRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Insert(InsertChangeShiftRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = null;
            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
                if (commandResult.Code == 0)
                {
                    //SendMailForChangeShiftSuccessfully(model, requestId, false);
                    //var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
                    //var taskSendMail = new Task(() => this.SendMailForInsertLeaveSuccessfully(criteria, templateEmailPath));
                    //taskSendMail.Start();
                }
            }
            else
            {
                commandResult = new CommandResult() 
                {
                    Code = 1,
                    Message = Resources.Messages.InvalidInputData
                };
            }

            return commandResult;
        }

        /// <summary>
        /// Function to update change shift request.
        /// </summary>
        /// <param name="criteria">InsertLeaveRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Update(UpdateChangeShiftRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = null;
            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
                if (commandResult.Code == 0)
                {
                    //SendMailForChangeShiftSuccessfully(model, requestId, false);
                    //var templateEmailPath = HttpContext.Current.Server.MapPath(_LeaveRequestEmailTpl);
                    //var taskSendMail = new Task(() => this.SendMailForInsertLeaveSuccessfully(criteria, templateEmailPath));
                    //taskSendMail.Start();
                }
            }
            else
            {
                commandResult = new CommandResult()
                {
                    Code = 1,
                    Message = Resources.Messages.InvalidInputData
                };
            }

            return commandResult;
        }

        public IList<ChangeShiftDetailModel> GetChangeShiftRequestDetailById(GetChangeShiftRequestDetailByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var detail = new List<ChangeShiftDetailModel>();
            if (criteria.IsValid())
            {
                detail = GetData<ChangeShiftDetailModel>(criteria).ToList();
            }

            return detail;
        }

        /// <summary>
        /// Sends the mail for change shift request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="isUpdate">If set to <c>true</c> [is update].</param>
        /// <param name="templateEmailPath">The template email path.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCC">The mail cc.</param>
        /// <param name="sendToNickName">Name of the send to nick.</param>
        private void SendMailForChangeShiftRequest(
            ChangeShiftViewModel model,
            long requestId,
            bool isUpdate,
            string templateEmailPath,
            string mailTo,
            string mailCC,
            string sendToNickName)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = int.Parse(model.Employee) };
            var employee = this._employeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            var subject = (isUpdate ? Messages.UpdatedChangeShiftEmailSubject : Messages.ChangeShiftEmailSubject)
                + " " + employee.DisplayName + " on " + model.FromDate.ToDateMonthYearFormatedDate();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            var content = new
            {
                Leader = sendToNickName,
                Member = model.Employee,
                RequestId = requestId,
                ForDate = GetMessageDateString(model.FromDate, model.ToDate),
                Remark = model.Remark,
                Url = this._baseUrl
            };
            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                mailCC,
                subject,
                body);
        }

        /// <summary>
        /// Function to send mail for saved success for late in, early out request.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is update or insert.</param>
        private void SendMailForChangeShiftSuccessfully(ChangeShiftViewModel model, long requestId, bool isUpdate)
        {
            EmailModel email = GetMailToAndMailCC(requestId, isUpdate ? (int)SettingsSendMail.Edit : (int)SettingsSendMail.New);
            if (!string.IsNullOrWhiteSpace(email.SendTo))
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_ChangeShiftTmp);
                var taskSendMail = new Task(()
                    => this.SendMailForChangeShiftRequest(model, requestId, isUpdate, templateEmailPath, email.SendTo, email.SendCc, email.SendToNickName));
                taskSendMail.Start();
            }
        }
    }
}
