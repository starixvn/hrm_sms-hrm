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
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Check In Out Request Service.
    /// </summary>
    public class CheckInOutRequestService : RequestService, ICheckInOutRequestService
    {
        private const string _CheckInOutTmp = "~/app_data/emails/CheckInOut.Request.cshtml";

        /// <summary>
        /// Function to insert check in/out request.
        /// </summary>
        /// <param name="criteria">InsertCheckInOutRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Insert(InsertCheckInOutRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria, hasGetOutputParams: true);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_CheckInOutTmp);
                    var taskSendMail = new Task(() => this.SendMailForInsertCheckInOutSuccessfully(criteria, templateEmailPath));
                    taskSendMail.Start();
                }
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Resources.Messages.InvalidInputData
                };
            }

            return result;
        }

        /// <summary>
        /// Function to update check in/out request.
        /// </summary>
        /// <param name="criteria">UpdateCheckInOutRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Update(UpdateCheckInOutRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_CheckInOutTmp);
                    var taskSendMail = new Task(() => this.SendMailForUpdateCheckInOutSuccessfully(criteria, templateEmailPath));
                    taskSendMail.Start();
                }
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Resources.Messages.InvalidInputData
                };
            }

            return result;
        }

        /// <summary>
        /// Function to send mail for inserted success for check in out request.
        /// </summary>
        /// <param name="criteria">InsertCheckInOutRequest Criteria.</param>
        /// <param name="templateEmailPath">Template Email Path.</param>
        private void SendMailForInsertCheckInOutSuccessfully(InsertCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.CheckInOutEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            var content = new
            {
                Leader = email.SendToNickName,
                Member = employee.NickName,
                Type = criteria.RequestType == (short)RequestType.CheckIn
                    ? RequestType.CheckIn.ToString().ToLower()
                    : RequestType.CheckOut.ToString().ToLower(),
                RequestId = criteria.RequestId,
                ForDate = criteria.OnDate.ToDateMonthYearFormatedDate(),
                Time = criteria.CheckTime.ToShortFormatedDate(),
                Shift = criteria.Shift,
                Url = this.BaseLink
            };
            email.Body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }

        /// <summary>
        /// Function to send mail for updated success for check in out request.
        /// </summary>
        /// <param name="criteria">UpdateCheckInOutRequest Criteria.</param>
        /// <param name="templateEmailPath">Template Email Path.</param>
        private void SendMailForUpdateCheckInOutSuccessfully(UpdateCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.UpdatedCheckInOutEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            var content = new
            {
                Leader = email.SendToNickName,
                Member = employee.NickName,
                Type = criteria.RequestType == (short)RequestType.CheckIn
                    ? RequestType.CheckIn.ToString().ToLower()
                    : RequestType.CheckOut.ToString().ToLower(),
                RequestId = criteria.RequestId,
                ForDate = criteria.OnDate.ToDateMonthYearFormatedDate(),
                Time = criteria.CheckTime.ToShortFormatedDate(),
                Shift = criteria.Shift,
                Url = this.BaseLink
            };
            email.Body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }
    }
}
