// -----------------------------------------------------------------------
// <copyright file="LateInEarlyOutRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
    /// LateInEarlyOutRequest Service.
    /// </summary>
    public class LateInEarlyOutRequestService : RequestService, ILateInEarlyOutRequestService
    {
        private const string _LateInEarlyOutEmailTpl = "~/app_data/emails/LateInEarlyOut.Request.cshtml";
        private readonly IEmailService _emailService;
        private readonly ILateInEarlyOutRepository _lateInEarlyOutRepository;
        private string _baseUrl;

        public LateInEarlyOutRequestService()
        {
            this._emailService = new EmailService();
            this._lateInEarlyOutRepository = new LateInEarlyOutRepository();
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        public LateInEarlyOutRequestService(
            IEmailService emailService,
            ILateInEarlyOutRepository lateInEarlyOutRepository)
        {
            this._emailService = emailService;
            this._lateInEarlyOutRepository = lateInEarlyOutRepository;
            this._baseUrl = "http://" + HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// Insert Late In, Early Out request.
        /// </summary>
        /// <param name="modelState">ModelState Dictionary.</param>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>Result of insert.</returns>
        public bool Insert(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
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

            var requestId = this._lateInEarlyOutRepository.Insert(model, out errorCode, out message);
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
        public bool Update(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
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
                var updated = this._lateInEarlyOutRepository.Update(model, out errorCode, out message);

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

        public RequestEntity GetRequestById(long id)
        {
            if (id > 0)
            {
                return this._lateInEarlyOutRepository.GetRequestById(id, (int)RequestFilter.LateInEarlyOut);
            }

            return null;
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
