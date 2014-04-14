// --------------------------------------------------------------------------
// <copyright file="KeepInViewRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
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
    /// RequestService Class.
    /// </summary>
    public class KeepInViewRequestService : RequestService, IKeepInViewRequestService
    {
        private const string _KeepInViewRequestEmailTpl = "~/app_data/emails/KeepInViewRequest.cshtml";
        private readonly IEmailService _emailService;
        private readonly IKeepInViewRequestRepository _keepInViewRequestRepository;

        public KeepInViewRequestService()
        {
            this._keepInViewRequestRepository = new KeepInViewRequestRepository();
            this._emailService = new EmailService();
        }

        public KeepInViewRequestService(
            IEmailService emailService,
            IKeepInViewRequestRepository keepInViewRequestRepository)
        {
            this._emailService = emailService;
            this._keepInViewRequestRepository = keepInViewRequestRepository;
        }

        /// <summary>
        /// Inserts the keep in view request.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The keep in view request view model.</param>
        /// <param name="isTopManager">If set to <c>true</c> [is top manager].</param>
        /// <returns>
        /// Indicate the insert action is successful or not.
        /// </returns>
        public bool Insert(ModelStateDictionary modelState, KeepInViewRequestViewModel model, bool isTopManager)
        {
            if (!modelState.IsValid)
            {
                return false;
            }

            if (!this.CheckIsValidKeepInTimeRequest(modelState, model, isTopManager))
            {
                return false;
            }

            int errorCode;
            string message;
            var requestId = this._keepInViewRequestRepository.Insert(model, out errorCode, out message);

            if (errorCode == 0)
            {
                SendMailForSuccessfulKeepInViewRequest(model, requestId, isUpdate: false);
                return true;
            }

            modelState.AddModelError(string.Empty, message);
            return false;
        }

        /// <summary>
        /// Updates the keep in view request.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="model">The keep in view request view model.</param>
        /// <param name="isTopManager">If set to <c>true</c> [is top manager].</param>
        /// <returns>
        /// Indicate the update action is successful or not.
        /// </returns>
        public bool Update(ModelStateDictionary modelState, KeepInViewRequestViewModel model, bool isTopManager)
        {
            if (!modelState.IsValid)
            {
                return false;
            }

            if (!this.CheckIsValidKeepInTimeRequest(modelState, model, isTopManager))
            {
                return false;
            }

            int errorCode;
            string message;
            var updated = this._keepInViewRequestRepository.Update(model, out errorCode, out message);

            if (updated)
            {
                this.SendMailForSuccessfulKeepInViewRequest(model, model.Id, isUpdate: true);
                return true;
            }

            modelState.AddModelError(string.Empty, message);
            return false;
        }

        /// <summary>
        /// Gets the keep in view by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The RequestEntity entity.
        /// </returns>
        public RequestEntity GetRequestById(long id)
        {
            if (id > 0)
            {
                return this._keepInViewRequestRepository.GetRequestById(id, (int)RequestFilter.KeepInView);
            }

            return null;
        }

        /// <summary>
        /// Checks the is valid keep in time request.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="keepInViewRequestViewModel">The keep in view request view model.</param>
        /// <param name="isTopManager">If set to <c>true</c> [is top manager].</param>
        /// <returns>
        /// Indicate the keep in view request is valid or not.
        /// </returns>
        private bool CheckIsValidKeepInTimeRequest(ModelStateDictionary modelState, KeepInViewRequestViewModel keepInViewRequestViewModel, bool isTopManager)
        {
            ////if (keepInViewRequestViewModel.ShiftId <= 0)
            ////{
            ////    modelState.AddModelError("NoShiftArrangedForThisDate", Messages.NoShiftArrangedForThisDate);
            ////    return false;
            ////}

            if (!isTopManager && (keepInViewRequestViewModel.ApproverId == null || keepInViewRequestViewModel.VerifierId == null))
            {
                modelState.AddModelError("VerifierOrApproverIsRequired", Messages.VerifierOrApproverIsRequired);
                return false;
            }

            if (keepInViewRequestViewModel.HalfDay && !keepInViewRequestViewModel.CrossDay)
            {
                string[] fromTimeArray = keepInViewRequestViewModel.FromTime.Split(':');
                string[] toTimeArray = keepInViewRequestViewModel.ToTime.Split(':');

                if (_keepInViewRequestRepository.ConvertHourFromStringToDouble(fromTimeArray[0]) > _keepInViewRequestRepository.ConvertHourFromStringToDouble(toTimeArray[0]))
                {
                    modelState.AddModelError("FromTimeShouldBeLessThanToTime", Messages.FromTimeShouldBeLessThanToTime);
                    return false;
                }
                else if (_keepInViewRequestRepository.ConvertHourFromStringToDouble(fromTimeArray[0]) 
                    == _keepInViewRequestRepository.ConvertHourFromStringToDouble(toTimeArray[0])
                    && Convert.ToInt32(fromTimeArray[1]) > Convert.ToInt32(toTimeArray[1]))
                {
                    modelState.AddModelError("FromTimeShouldBeLessThanToTime", Messages.FromTimeShouldBeLessThanToTime);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sends the mail for successful keep in view request.
        /// </summary>
        /// <param name="keepInViewRequestViewModel">The keep in view request view model.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="isUpdate">If set to <c>true</c> [is update].</param>
        private void SendMailForSuccessfulKeepInViewRequest(KeepInViewRequestViewModel keepInViewRequestViewModel, long requestId, bool isUpdate)
        {
            var mailTo = string.Empty;
            var mailCC = string.Empty;
            var sendToNickName = string.Empty;

            GetMailToAndMailCC(requestId, isUpdate ? (int)SettingsSendMail.Edit : (int)SettingsSendMail.New, ref mailTo, ref mailCC, ref sendToNickName);

            if (!string.IsNullOrWhiteSpace(mailTo))
            {
                var templateEmailPath = HttpContext.Current.Server.MapPath(_KeepInViewRequestEmailTpl);
                var taskSendMail = new Task(() => this.ProcessSendMail(keepInViewRequestViewModel, sendToNickName, isUpdate, templateEmailPath, mailTo, mailCC));
                taskSendMail.Start();
            }
        }

        /// <summary>
        /// Function to send mail for leave request.
        /// </summary>
        /// <param name="keepInViewRequestViewModel">LeaveRequest ViewModel.</param>
        /// <param name="receiverNickName">Nick name of person email send to.</param>
        /// <param name="isUpdate">Is Update or insert.</param>
        /// <param name="emailTemplatePath">Path of email template.</param>
        /// <param name="mailTo">MailTo List.</param>
        /// <param name="mailCc">MailCC List.</param>
        private void ProcessSendMail(
            KeepInViewRequestViewModel keepInViewRequestViewModel,
            string receiverNickName,
            bool isUpdate,
            string emailTemplatePath,
            string mailTo,
            string mailCc)
        {
            var bodyTemplate = Utilities.ReadTextFile(emailTemplatePath);
            string subject = (isUpdate ? Messages.UpdatedKeepInViewEmailSubject : Messages.InsertedKeepInViewEmailSubject) + " " + keepInViewRequestViewModel.Employee;
            subject += " on " + keepInViewRequestViewModel.Date.ToDateMonthYearFormatedDate();

            var content = new
            {
                LeaderName = receiverNickName,
                MemberName = keepInViewRequestViewModel.DisplayName,
                RequestId = keepInViewRequestViewModel.Id,
                Date = keepInViewRequestViewModel.Date.ToDateMonthYearFormatedDate(),
                Shift = keepInViewRequestViewModel.Shift,
                Remark = keepInViewRequestViewModel.Remark,
                Url = this.BaseUrl
            };

            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._emailService.SendEmail(
                ConfigSettings.EmailSetting,
                mailTo,
                mailCc,
                subject,
                body);
        }
    }
}