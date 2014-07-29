// -----------------------------------------------------------------------
// <copyright file="ChangeShiftRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Change Shift Request Service.
    /// </summary>
    public class ChangeShiftRequestService : RequestService, IChangeShiftRequestService
    {
        private const string _ChangeShiftTmp = "~/app_data/emails/ChangeShift.Request.cshtml";
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly IChangeShiftRepository _changeShiftRepository;
        private string _baseUrl;

        public ChangeShiftRequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._emailService = new EmailService();
            this._changeShiftRepository = new ChangeShiftRepository();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public ChangeShiftRequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            IChangeShiftRepository changeShiftRepository)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._changeShiftRepository = changeShiftRepository;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Insert Change Shift request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">ChangeShift ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool Insert(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                var requestId = this._changeShiftRepository.Insert(model, out errorCode, out message);
                if (errorCode == 0)
                {
                    SendMailForChangeShiftSuccessfully(model, requestId, false);
                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Update Change Shift request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">ChangeShift ViewModel.</param>
        /// <returns>Result of update.</returns>
        public bool Update(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                var result = this._changeShiftRepository.Update(model, out errorCode, out message);
                if (result)
                {
                    SendMailForChangeShiftSuccessfully(model, model.Id, true);
                }

                return true;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public RequestEntity GetRequestById(long id)
        {
            if (id > 0)
            {
                return this._changeShiftRepository.GetRequestById(id, (int)RequestFilter.ChangeShift);
            }

            return null;
        }

        public List<ShiftDetails> GetChangeShiftDetailsById(long id)
        {
            if (id > 0)
            {
                return this._changeShiftRepository.GetChangeShiftDetailsById(id);
            }

            return null;
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
                Member = model.DisplayName,
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
