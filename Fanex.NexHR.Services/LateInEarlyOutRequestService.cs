// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.4">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// LateInEarlyOutRequest Service.
    /// </summary>
    public class LateInEarlyOutRequestService : RequestService, ILateInEarlyOutRequestService
    {
        private const string _LateInEarlyOutEmailTpl = "~/app_data/emails/LateInEarlyOut.Request.cshtml";

        /// <summary>
        /// Function to insert late in, early out request.
        /// </summary>
        /// <param name="criteria">InsertLateInEarlyOutRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Insert(InsertLateInEarlyOutRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_LateInEarlyOutEmailTpl);
                    var taskSendMail = new Task(() => this.SendMailForInsertLateInEarlyOutSuccessfully(criteria, templateEmailPath));
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
        public CommandResult Update(UpdateLateInEarlyOutRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_LateInEarlyOutEmailTpl);
                    var taskSendMail = new Task(() => this.SendMailForUpdateLateInEarlyOutSuccessfully(criteria, templateEmailPath));
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
        private void SendMailForInsertLateInEarlyOutSuccessfully(InsertLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this._EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            var subject = Messages.LateInEarlyOutEmailSubject + " " + employee.NickName
                + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            var content = new
            {
                Leader = email.SendToNickName,
                Member = employee.DisplayName,
                Type = criteria.RequestType == (short)RequestType.LateIn
                    ? "a " + Resources.Labels.LateIn.ToLower()
                    : "an " + Resources.Labels.EarlyOut.ToLower(),
                RequestId = criteria.RequestId,
                ForDate = criteria.OnDate.ToDateMonthYearFormatedDate(),
                Minute = criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                Remark = criteria.Remark,
                Url = this.BaseUrl
            };
            email.Body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._EmailService.SendEmail(
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
        private void SendMailForUpdateLateInEarlyOutSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this._EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            var subject = Messages.UpdatedLateInEarlyOutEmailSubject + " " + employee.NickName
                + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            var content = new
            {
                Leader = email.SendToNickName,
                Member = employee.DisplayName,
                Type = criteria.RequestType == (short)RequestType.LateIn
                    ? "a " + Resources.Labels.LateIn.ToLower()
                    : "an " + Resources.Labels.EarlyOut.ToLower(),
                RequestId = criteria.RequestId,
                ForDate = criteria.OnDate.ToDateMonthYearFormatedDate(),
                Minute = criteria.RequestType == (short)RequestType.LateIn ? criteria.LateInMins : criteria.EarlyOutMins,
                Remark = criteria.Remark,
                Url = this.BaseUrl
            };
            email.Body = RazorEngine.Razor.Parse(bodyTemplate, content);
            this._EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }
    }
}
