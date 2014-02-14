// --------------------------------------------------------------------------
// <copyright file="CheckInOutRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

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
    public class CheckInOutRequestService : RequestService, ICheckInOutRequestService
    {
        private const string _CheckInOutTmp = "~/app_data/emails/CheckInOut.Request.cshtml";
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly IRequestRepository _requestRepository;
        private readonly ICheckInOutRepository _checkInOutRepository;
        private string _baseUrl;

        public CheckInOutRequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._requestRepository = new RequestRepository();
            this._emailService = new EmailService();
            this._checkInOutRepository = new CheckInOutRepository();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public CheckInOutRequestService(
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            IRequestRepository requestRepository,
            ICheckInOutRepository checkInOutRepository)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._requestRepository = requestRepository;
            this._checkInOutRepository = checkInOutRepository;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Checks the in out.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The CheckInOutRequestViewModel.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool InsertCheckInOutRequest(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                model.AttendantDate = model.AttendantDate + new TimeSpan(model.Hour, model.Minute, 0);
                int errorCode;
                string message;
                var requestId = this._checkInOutRepository.InsertCheckInOutRequest(
                    model.EmployeeId,
                    model.Type,
                    model.Remark,
                    model.ForDate,
                    DateTime.Now,
                    model.By,
                    model.VerifierId,
                    model.ApproverId,
                    model.RefNO,
                    model.AttendantDate,
                    out errorCode,
                    out message);

                if (errorCode != 0)
                {
                    modelState.AddModelError(string.Empty, message);
                }
                else
                {
                    SendEmailForCheckInOutSuccessfully(model, requestId, isUpdate: false);
                    return true;
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Function to get check in, check out request.
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
        public IList<RequestEntity> GetCheckInOutRequests(
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
                    entityList = this._checkInOutRepository.GetCheckInOutRequests(role, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        public RequestEntity GetCheckInOutRequestById(long id)
        {
            if (id > 0)
            {
                return this._checkInOutRepository.GetCheckInOutRequestById(id);
            }

            return null;
        }

        /// <summary>
        /// Function to get Check in, check out request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoricCheckInOutRequests(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._checkInOutRepository.GetCheckInOutRequestHistory(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Updates the check in out.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The CheckInOutRequestViewModel.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UpdateCheckInOutRequest(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                model.AttendantDate = model.AttendantDate + new TimeSpan(model.Hour, model.Minute, 0);

                var updated = this._checkInOutRepository.UpdateCheckInOutRequest(
                     Convert.ToInt32(model.Id),
                     model.EmployeeId,
                     model.Type,
                     model.Remark,
                     model.ForDate,
                     model.VerifierId,
                     model.ApproverId,
                     DateTime.Now,
                     model.EmployeeId,
                     model.RefNO,
                     model.AttendantDate);

                if (updated)
                {
                    SendEmailForCheckInOutSuccessfully(model, model.Id, isUpdate: true);
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        /// <summary>
        /// Sends the email for check in out success.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="isUpdate">If set to <c>true</c> [is updated].</param>
        private void SendEmailForCheckInOutSuccessfully(CheckInOutRequestViewModel model, long requestId, bool isUpdate)
        {
            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;

            GetMailToAndMailCC(requestId, isUpdate ? (int)SettingsSendMail.Edit : (int)SettingsSendMail.New, ref mailTo, ref mailCC, ref sendToNickName);

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_CheckInOutTmp);
                var taskSendMail = new Task(() => this.SendMailForCheckInOutRequest(model, requestId, isUpdate, templateEmailPath, mailTo, mailCC, sendToNickName));
                taskSendMail.Start();
            }
        }

        /// <summary>
        /// Sends the mail for check in out request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="isUpdate">If set to <c>true</c> [is update].</param>
        /// <param name="templateEmailPath">The template email path.</param>
        /// <param name="mailTo">The mail to.</param>
        /// <param name="mailCC">The mail cc.</param>
        /// <param name="sendToNickName">Name of the send to nick.</param>
        private void SendMailForCheckInOutRequest(
            CheckInOutRequestViewModel model,
            long requestId,
            bool isUpdate,
            string templateEmailPath,
            string mailTo,
            string mailCC,
            string sendToNickName)
        {
            var subject = (isUpdate ? Messages.UpdatedCheckInOutEmailSubject : Messages.CheckInOutEmailSubject)
                + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate();

            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);

            var content = new
            {
                Leader = sendToNickName,
                Member = model.DisplayName,
                Type = model.Type == (short)RequestTypes.CheckIn
                    ? RequestTypes.CheckIn.EnumToString().ToLower()
                    : RequestTypes.CheckOut.EnumToString().ToLower(),
                RequestId = requestId,
                ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                Shift = model.Shift,
                Url = this._baseUrl
            };
            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._emailService.SendEmail(ConfigurationManager.AppSettings["Email"], mailTo, mailCC, subject, body);
        }
    }
}
