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
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Implement;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    public class CheckInOutRequestService : RequestService, ICheckInOutRequestService
    {
        private const string CheckInOutTmp = "~/app_data/emails/CheckInOut.Request.cshtml";

        private readonly ICheckInOutRequestRepository _checkInOutRequestRepository;

        public CheckInOutRequestService()
            : this(null)
        {
        }

        public CheckInOutRequestService(ICheckInOutRequestRepository checkInOutRequestRepository)
        {
            _checkInOutRequestRepository = checkInOutRequestRepository ?? new CheckInOutRequestRepository();
        }

        public CommandResult Insert(InsertCheckInOutRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Insert(criteria, true);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
                    var taskSendMail = new Task(() => this.SendMailForInsertCheckInOutSuccessfully(criteria, templateEmailPath));
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

        public CommandResult Update(UpdateCheckInOutRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Update(criteria);
                if (result != null && result.Code == 0)
                {
                    var templateEmailPath = HttpContext.Current.Server.MapPath(CheckInOutTmp);
                    var taskSendMail = new Task(() => this.SendMailForUpdateCheckInOutSuccessfully(criteria, templateEmailPath));
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

        public RequestEntity GetCheckInOutRequestToIgnoreById(GetCheckInOutRequestToIgnoreByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            RequestEntity request = null;

            if (criteria.IsValid())
            {
                request = _checkInOutRequestRepository.GetCheckInOutRequestToIgnoreById(criteria);
            }

            return request;
        }

        private void SendMailForInsertCheckInOutSuccessfully(InsertCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.New);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.CheckInOutEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.CheckTime.ToShortTime(),
                    criteria.Remark);

            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }

        private void SendMailForUpdateCheckInOutSuccessfully(UpdateCheckInOutRequestCriteria criteria, string templateEmailPath)
        {
            EmailModel email = GetMailToAndMailCC(criteria.RequestId, (int)SettingsSendMail.Edit);

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria()
            {
                Empid = criteria.EmpId
            };
            var employee = this.EmployeeRepository.GetEmployeeById(getEmployeeByIdCriteria);
            email.Subject = Messages.UpdatedCheckInOutEmailSubject + " " + employee.NickName + " on " + criteria.OnDate.ToDateMonthYearFormatedDate();
            email.Body = Utilities.ReadTextFile(templateEmailPath);
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                employee.NickName,
                criteria.RequestType == (short)RequestType.CheckIn ? RequestType.CheckIn.ToString() : RequestType.CheckOut.ToString(),
                    criteria.OnDate.ToDateMonthYearFormatedDate(),
                    criteria.Shift,
                    criteria.CheckTime.ToShortTime(),
                    criteria.Remark);

            this.EmailService.SendEmail(
                ConfigSettings.EmailSetting,
                email.SendTo,
                email.SendCc,
                email.Subject,
                email.Body);
        }
    }
}