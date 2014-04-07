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
    using System.Collections.Generic;
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
            IEmployeeRepository employeeRepository,
            IEmailService emailService,
            ILeaveTypeService leaveTypeService,
            IKeepInViewRequestRepository keepInViewRequestRepository,
            IShiftService shiftService,
            IRequestRepository requestRepository)
        {
            this._emailService = emailService;
            this._keepInViewRequestRepository = keepInViewRequestRepository;
        }

        /// <summary>
        /// Inserts the keep in view request.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="keepInViewRequestViewModel">The keep in view request view model.</param>
        /// <param name="isTopManager">If set to <c>true</c> [is top manager].</param>
        /// <returns>
        /// Indicate the insert action is successful or not.
        /// </returns>
        public bool InsertKeepInViewRequest(ModelStateDictionary modelState, KeepInViewRequestViewModel keepInViewRequestViewModel, bool isTopManager)
        {
            if (!modelState.IsValid)
            {
                return false;
            }

            if (!this.CheckIsValidKeepInTimeRequest(modelState, keepInViewRequestViewModel, isTopManager))
            {
                return false;
            }

            int errorCode;
            string message;
            DateTime? fromTime = null;
            DateTime? toTime = null;
            this.ConvertFromTimeAndToTimeFromStringFormatToDateTimeFormat(keepInViewRequestViewModel, ref fromTime, ref toTime);

            var requestId = this._keepInViewRequestRepository.InsertKeepInViewRequest(
                keepInViewRequestViewModel.EmployeeId,
                (short)RequestTypes.KeepInView,
                keepInViewRequestViewModel.Remark,
                keepInViewRequestViewModel.Date,
                keepInViewRequestViewModel.HalfDay,
                keepInViewRequestViewModel.ShiftId,
                fromTime,
                toTime,
                keepInViewRequestViewModel.By,
                DateTime.Now,
                keepInViewRequestViewModel.VerifierId,
                keepInViewRequestViewModel.ApproverId,
                keepInViewRequestViewModel.RefNo,
                out errorCode,
                out message);

            if (errorCode == 0)
            {
                SendMailForSuccessfulKeepInViewRequest(keepInViewRequestViewModel, requestId, isUpdate: false);
                return true;
            }

            modelState.AddModelError(string.Empty, message);
            return false;
        }

        /// <summary>
        /// Updates the keep in view request.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="keepInViewRequestViewModel">The keep in view request view model.</param>
        /// <param name="isTopManager">If set to <c>true</c> [is top manager].</param>
        /// <returns>
        /// Indicate the update action is successful or not.
        /// </returns>
        public bool UpdateKeepInViewRequest(ModelStateDictionary modelState, KeepInViewRequestViewModel keepInViewRequestViewModel, bool isTopManager)
        {
            if (!modelState.IsValid)
            {
                return false;
            }

            if (!this.CheckIsValidKeepInTimeRequest(modelState, keepInViewRequestViewModel, isTopManager))
            {
                return false;
            }

            int errorCode;
            string message;
            DateTime? fromTime = null;
            DateTime? toTime = null;
            this.ConvertFromTimeAndToTimeFromStringFormatToDateTimeFormat(keepInViewRequestViewModel, ref fromTime, ref toTime);

            var updated = this._keepInViewRequestRepository.UpdateKeepInViewRequest(
                keepInViewRequestViewModel.Id,
                keepInViewRequestViewModel.EmployeeId,
                (short)RequestTypes.KeepInView,
                keepInViewRequestViewModel.Remark,
                keepInViewRequestViewModel.Date,
                keepInViewRequestViewModel.HalfDay,
                keepInViewRequestViewModel.ShiftId,
                fromTime,
                toTime,
                keepInViewRequestViewModel.EmployeeId,
                DateTime.Now,
                keepInViewRequestViewModel.VerifierId,
                keepInViewRequestViewModel.ApproverId,
                keepInViewRequestViewModel.RefNo,
                out errorCode,
                out message);

            if (updated)
            {
                this.SendMailForSuccessfulKeepInViewRequest(keepInViewRequestViewModel, keepInViewRequestViewModel.Id, isUpdate: true);
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
        public RequestEntity GetKeepInViewRequestById(long id)
        {
            if (id > 0)
            {
                return this._keepInViewRequestRepository.GetKeepInViewRequestById(id);
            }

            return null;
        }

        /// <summary>
        /// Gets the keep in view request.
        /// </summary>
        /// <param name="role">The role identifier.</param>
        /// <param name="fromDate">Get result by From Date.</param>
        /// <param name="toDate">Get result by To Date.</param>
        /// <param name="groupIds">The group ids.</param>
        /// <param name="isDailyRequest">If set to <c>true</c> [is daily request].</param>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="status">Get result by Status.</param>
        /// <param name="loginId">The login identifier.</param>
        /// <returns>The list of RequestEntity.</returns>
        public IList<RequestEntity> GetKeepInViewRequests(
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
                    entityList = this._keepInViewRequestRepository.GetKeepInViewRequests(role, fromDate, toDate, groupIds, isDailyRequest, employeeId, status, loginId);
                }
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Function to get Keep in view request history.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type Id.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetHistoricKeepInViewRequests(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            try
            {
                entityList = this._keepInViewRequestRepository.GetHistoricKeepInViewRequests(requestId, requestTypeId);
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message, ex);
            }

            return entityList;
        }

        /// <summary>
        /// Converts from time and to time from string format to date time format.
        /// </summary>
        /// <param name="keepInViewRequestViewModel">The keep in view request view model.</param>
        /// <param name="fromTime">From time for output.</param>
        /// <param name="toTime">To time for output.</param>
        private void ConvertFromTimeAndToTimeFromStringFormatToDateTimeFormat(
            KeepInViewRequestViewModel keepInViewRequestViewModel,
            ref DateTime? fromTime,
            ref DateTime? toTime)
        {
            if (keepInViewRequestViewModel.HalfDay)
            {
                string[] fromTimeArray = keepInViewRequestViewModel.FromTime.Split(':');
                string[] toTimeArray = keepInViewRequestViewModel.ToTime.Split(':');
                fromTime = new DateTime(keepInViewRequestViewModel.Date.Year, keepInViewRequestViewModel.Date.Month, keepInViewRequestViewModel.Date.Day);
                toTime = new DateTime(keepInViewRequestViewModel.Date.Year, keepInViewRequestViewModel.Date.Month, keepInViewRequestViewModel.Date.Day);

                fromTime = fromTime.Value.AddHours(this.ConvertHourFromStringToDouble(fromTimeArray[0])).AddMinutes(Convert.ToDouble(fromTimeArray[1]));

                if (keepInViewRequestViewModel.CrossDay)
                {
                    toTime = toTime.Value.AddDays(1);
                }

                toTime = toTime.Value.AddHours(this.ConvertHourFromStringToDouble(toTimeArray[0])).AddMinutes(Convert.ToDouble(toTimeArray[1]));
            }
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

                if (this.ConvertHourFromStringToDouble(fromTimeArray[0]) > this.ConvertHourFromStringToDouble(toTimeArray[0]))
                {
                    modelState.AddModelError("FromTimeShouldBeLessThanToTime", Messages.FromTimeShouldBeLessThanToTime);
                    return false;
                }
                else if (this.ConvertHourFromStringToDouble(fromTimeArray[0]) == this.ConvertHourFromStringToDouble(toTimeArray[0])
                    && Convert.ToInt32(fromTimeArray[1]) > Convert.ToInt32(toTimeArray[1]))
                {
                    modelState.AddModelError("FromTimeShouldBeLessThanToTime", Messages.FromTimeShouldBeLessThanToTime);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts the hour from string to double.
        /// </summary>
        /// <param name="hour">The input hour for converting.</param>
        /// <returns>The double format of hour.</returns>
        private double ConvertHourFromStringToDouble(string hour)
        {
            if (hour.StartsWith("0"))
            {
                hour = hour.Substring(1);
            }

            return Convert.ToDouble(hour);
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