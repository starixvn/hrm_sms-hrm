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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Employee;

    /// <summary>
    /// RequestService Class.
    /// </summary>
    public class KeepInViewRequestService : RequestService, IKeepInViewRequestService
    {
        private const string _KeepInViewRequestEmailTpl = "~/app_data/emails/KeepInViewRequest.cshtml";

        /// <summary>
        /// Function to insert keep in view request.
        /// </summary>
        /// <param name="criteria">InsertKeepInViewRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Insert(InsertKeepInViewRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria, hasGetOutputParams: true);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_KeepInViewRequestEmailTpl);
                    var taskSendMail = new Task(() => this.SendMailForInsertKeepInViewSuccessfully(criteria, templateEmailPath));
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
        /// Function to update keep in view request.
        /// </summary>
        /// <param name="criteria">UpdateKeepInViewRequest Criteria.</param>
        /// <returns>Command Result.</returns>
        public CommandResult Update(UpdateKeepInViewRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(_KeepInViewRequestEmailTpl);
                    var taskSendMail = new Task(() => this.SendMailForUpdateKeepInViewSuccessfully(criteria, templateEmailPath));
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
        private void SendMailForInsertKeepInViewSuccessfully(InsertKeepInViewRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.InsertedKeepInViewEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();

            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            var content = new
            {
                LeaderName = email.SendToNickName,
                MemberName = employee.NickName,
                RequestId = criteria.RequestId,
                Date = criteria.OnDate.ToDateMonthYearFormatedDate(),
                Shift = criteria.ShiftName,
                Remark = criteria.Remark,
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
        private void SendMailForUpdateKeepInViewSuccessfully(UpdateKeepInViewRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.UpdatedKeepInViewEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
            var content = new
            {
                LeaderName = email.SendToNickName,
                MemberName = employee.NickName,
                RequestId = criteria.RequestId,
                Date = criteria.OnDate.ToDateMonthYearFormatedDate(),
                Shift = criteria.ShiftName,
                Remark = criteria.Remark,
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