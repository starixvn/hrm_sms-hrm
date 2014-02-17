// -----------------------------------------------------------------------
// <copyright file="ChangeShiftRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
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
        private readonly IRequestRepository _requestRepository;
        private readonly IChangeShiftRepository _changeShiftRepository;
        private string _baseUrl;

        public ChangeShiftRequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._requestRepository = new RequestRepository();
            this._emailService = new EmailService();
            this._changeShiftRepository = new ChangeShiftRepository();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public ChangeShiftRequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            IRequestRepository requestRepository,
            IChangeShiftRepository changeShiftRepository)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._requestRepository = requestRepository;
            this._changeShiftRepository = changeShiftRepository;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Insert Change Shift request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">ChangeShift ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool InsertChangeShiftRequest(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                var requestId = this._changeShiftRepository.InsertChangeShiftRequest(model, out errorCode, out message);
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
        public bool UpdateChangeShiftRequest(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;
                var updated = this._changeShiftRepository.UpdateChangeShiftRequest(model, out errorCode, out message);
                if (updated == 0)
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

        /// <summary>
        /// Function to get change shift request.
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
        public IList<RequestEntity> GetChangeShiftRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? loginId = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                if (fromDate <= toDate)
                {
                    entityList = this._changeShiftRepository.GetChangeShiftRequests(role, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        public RequestEntity GetChangeShiftRequestById(long id)
        {
            if (id > 0)
            {
                return this._changeShiftRepository.GetChangeShiftRequestById(id);
            }

            return null;
        }

        public List<ShiftDetails> GetShiftDetailsById(long id)
        {
            if (id > 0)
            {
                return this._changeShiftRepository.GetShiftDetailsById(id);
            }

            return null;
        }

        /// <summary>
        /// Function to get Change shift request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoricChangeShiftRequests(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._changeShiftRepository.GetHistoricChangeShiftRequests(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
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
            var employee = this._employeeRepository.GetEmployeeById(int.Parse(model.Employee));
            var subject = (isUpdate ? Messages.UpdatedChangeShiftEmailSubject : Messages.ChangeShiftEmailSubject)
                + " " + employee.DisplayName + " on " + model.FromDate.ToDateMonthYearFormatedDate();

            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);

            var content = new
            {
                Leader = sendToNickName,
                Member = model.DisplayName,
                RequestId = requestId,
                FromDate = model.FromDate.ToDateMonthYearFormatedDate(),
                ToDate = model.ToDate.ToDateMonthYearFormatedDate(),
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
            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;

            GetMailToAndMailCC(requestId, isUpdate ? (int)SettingsSendMail.Edit : (int)SettingsSendMail.New, ref mailTo, ref mailCC, ref sendToNickName);

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_ChangeShiftTmp);
                var taskSendMail = new Task(() => this.SendMailForChangeShiftRequest(model, requestId, isUpdate, templateEmailPath, mailTo, mailCC, sendToNickName));
                taskSendMail.Start();
            }
        }
    }
}
