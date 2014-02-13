// -----------------------------------------------------------------------
// <copyright file="LateInEarlyOutRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LateInEarlyOutRequestService : RequestService, ILateInEarlyOutRequestService
    {
        private const string _LateInEarlyOutEmailTpl = "~/app_data/emails/LateInEarlyOut.Request.cshtml";
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly IRequestRepository _requestRepository;
        private readonly ILateInEarlyOutRepository _lateInEarlyOutRepository;
        private string _baseUrl;

        public LateInEarlyOutRequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._requestRepository = new RequestRepository();
            this._emailService = new EmailService();
            this._lateInEarlyOutRepository = new LateInEarlyOutRepository();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public LateInEarlyOutRequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            IRequestRepository requestRepository,
            ILateInEarlyOutRepository lateInEarlyOutRepository)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._requestRepository = requestRepository;
            this._lateInEarlyOutRepository = lateInEarlyOutRepository;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Insert Late In, Early Out request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool InsertLateInEarlyOutRequest(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
        {
            int errorCode;
            string message;
            int? lateInMins = model.Type == (short)RequestTypes.LateIn ? (int?)model.Minute : null;
            int? earlyOutMins = model.Type == (short)RequestTypes.EarlyOut ? (int?)model.Minute : null;
            if (!string.IsNullOrEmpty(model.FromTime) && !string.IsNullOrEmpty(model.ToTime))
            {
                model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
            }

            var requestId = this._lateInEarlyOutRepository.InsertLateInEarlyOutRequest(
                model.EmployeeId,
                model.Type,
                model.Remark,
                DateTime.Now,
                model.By,
                model.VerifierId,
                model.ApproverId,
                out errorCode,
                out message,
                model.Date,
                lateInMins: lateInMins,
                earlyOutMins: earlyOutMins);
            if (errorCode == 0)
            {
                SendMailForLateInEarlyOutSuccessfully(model, requestId, isUpdate: false);
                return true;
            }
            else
            {
                modelState.AddModelError(string.Empty, message);
            }

            return false;
        }

        /// <summary>
        /// Update Late In, Early Out request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>Result of update.</returns>
        public bool UpdateLateInEarlyOutRequest(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                int? lateInMins = model.Type == (short)RequestTypes.LateIn ? (int?)model.Minute : null;
                int? earlyOutMins = model.Type == (short)RequestTypes.EarlyOut ? (int?)model.Minute : null;
                if (!string.IsNullOrEmpty(model.FromTime) && !string.IsNullOrEmpty(model.ToTime))
                {
                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }

                int errorCode;
                string message;
                var updated = this._lateInEarlyOutRepository.UpdateLateInEarlyOutRequest(
                    Convert.ToInt32(model.Id),
                    model.EmployeeId,
                    model.Type,
                    model.Remark,
                    model.Date,
                    model.VerifierId,
                    model.ApproverId,
                    DateTime.Now,
                    model.EmployeeId,
                    out errorCode,
                    out message,
                    lateInMins: lateInMins,
                    earlyOutMins: earlyOutMins);

                if (updated)
                {
                    SendMailForLateInEarlyOutSuccessfully(model, model.Id, isUpdate: true);
                    return updated;
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
        /// Function to get late in, early out request.
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
        public IList<RequestEntity> GetLateInEarlyOutRequests(
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
                    entityList = this._lateInEarlyOutRepository.GetLateInEarlyOutRequests(role, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        public RequestEntity GetLateInEarlyOutRequestById(long id)
        {
            if (id > 0)
            {
                return this._lateInEarlyOutRepository.GetLateInEarlyOutRequestById(id);
            }

            return null;
        }

        /// <summary>
        /// Function to get Late in, early out request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoricLateInEarlyOutRequests(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._lateInEarlyOutRepository.GetHistoricLateInEarlyOutRequests(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to send mail for saved success for late in, early out request.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="isUpdate">Is update or insert.</param>
        private void SendMailForLateInEarlyOutSuccessfully(LateInEarlyOutRequestViewModel model, long requestId, bool isUpdate)
        {
            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;

            GetMailToAndMailCC(requestId, isUpdate ? (int)SettingsSendMail.Edit : (int)SettingsSendMail.New, ref mailTo, ref mailCC, ref sendToNickName);

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_LateInEarlyOutEmailTpl);
                var taskSendMail = new Task(() => this.SendMailForLateInEarlyOutRequest(model, requestId, isUpdate, templateEmailPath, mailTo, mailCC, sendToNickName));
                taskSendMail.Start();
            }
        }

        /// <summary>
        /// Sends the mail for late in early out request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="isUpdate">If set to <c>true</c> [is update].</param>
        /// <param name="templateEmailPath">The template email path.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCC">The mail cc.</param>
        /// <param name="sendToNickName">Name of the send to nick.</param>
        private void SendMailForLateInEarlyOutRequest(
            LateInEarlyOutRequestViewModel model,
            long requestId,
            bool isUpdate,
            string templateEmailPath,
            string mailTo,
            string mailCC,
            string sendToNickName)
        {
            var forDate = GetStringDateForLateInEarlyOutRequest(model.FromDate, model.ToDate);
            var subject = (isUpdate ? Messages.UpdatedLateInEarlyOutEmailSubject : Messages.LateInEarlyOutEmailSubject) + " " + model.Employee
                + " on " + model.Date.ToDateMonthYearFormatedDate();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);

            var content = new
            {
                Leader = sendToNickName,
                Member = model.DisplayName,
                Type = model.Type == (short)RequestTypes.LateIn
                    ? "a " + Resources.Labels.LateIn.ToLower()
                    : "an " + Resources.Labels.EarlyOut.ToLower(),
                RequestId = requestId,
                ForDate = model.Date.ToDateMonthYearFormatedDate(),
                Minute = model.Minute,
                Remark = model.Remark,
                Url = this._baseUrl
            };
            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                mailTo,
                string.Empty,
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
