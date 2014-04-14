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
    /// Check In Out Request Service.
    /// </summary>
    public class CheckInOutRequestService : RequestService, ICheckInOutRequestService
    {
        private const string _CheckInOutTmp = "~/app_data/emails/CheckInOut.Request.cshtml";
        private readonly IEmailService _emailService;
        private readonly ICheckInOutRepository _checkInOutRepository;
        private string _baseUrl;

        public CheckInOutRequestService()
        {
            this._emailService = new EmailService();
            this._checkInOutRepository = new CheckInOutRepository();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public CheckInOutRequestService(
            IEmailService emailService,
            ICheckInOutRepository checkInOutRepository)
        {
            this._emailService = emailService;
            this._checkInOutRepository = checkInOutRepository;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Checks the in out.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The CheckInOutRequestViewModel.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Insert(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                model.AttendantDate = model.AttendantDate + new TimeSpan(model.Hour, model.Minute, 0);
                int errorCode;
                string message;
                var requestId = this._checkInOutRepository.Insert(model, out errorCode, out message);

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
        /// Updates the check in out.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The CheckInOutRequestViewModel.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Update(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                model.AttendantDate = model.AttendantDate + new TimeSpan(model.Hour, model.Minute, 0);
                int errorCode;
                string message;
                var updated = this._checkInOutRepository.Update(model, out errorCode, out message);

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

        public RequestEntity GetRequestById(long id)
        {
            if (id > 0)
            {
                return this._checkInOutRepository.GetRequestById(id, (int)RequestFilter.CheckInOut);
            }

            return null;
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
