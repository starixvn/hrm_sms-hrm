// --------------------------------------------------------------------------
// <copyright file="ChangeShiftRequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Change Shift Request Service.
    /// </summary>
    public class ChangeShiftRequestService : RequestService, IChangeShiftRequestService
    {
        private const string ChangeShiftTmp = "~/app_data/emails/ChangeShift.Request.cshtml";

        public CommandResult Insert(InsertChangeShiftRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria, true);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(ChangeShiftTmp);
                    var taskSendMail = new Task(() => this.SendMailForInsertChangeShiftSuccessfully(criteria, templateEmailPath));
                    taskSendMail.Start();
                }
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return result;
        }

        public CommandResult Update(UpdateChangeShiftRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(ChangeShiftTmp);
                    var taskSendMail = new Task(() => this.SendMailForUpdateChangeShiftSuccessfully(criteria, templateEmailPath));
                    taskSendMail.Start();
                }
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return result;
        }

        public IList<ChangeShiftDetailModel> GetChangeShiftRequestDetailById(GetChangeShiftRequestDetailByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var detail = new List<ChangeShiftDetailModel>();
            if (criteria.IsValid())
            {
                detail = GetData<ChangeShiftDetailModel>(criteria).ToList();
            }

            return detail;
        }

        private void SendMailForInsertChangeShiftSuccessfully(InsertChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.ChangeShiftEmailSubject + " " + employee.DisplayName + " " + GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark);
            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }

        private void SendMailForUpdateChangeShiftSuccessfully(UpdateChangeShiftRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.UpdatedChangeShiftEmailSubject + " " + employee.DisplayName + " " + GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            var forDate = GetMessageDateString(criteria.FromDate, criteria.ToDate);
            email.Body = string.Format(email.Body, email.SendToNickName, employee.NickName, forDate, criteria.Remark);
            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }
    }
}