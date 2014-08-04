// -----------------------------------------------------------------------
// <copyright file="OvertimeRequestService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Request.Overtime;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// OvertimeRequest Service.
    /// </summary>
    public class OvertimeRequestService : BaseService, IOvertimeRequestService
    {
        private readonly IOvertimeRequestRepository _overtimeRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public OvertimeRequestService()
        {
            this._overtimeRepository = new OvertimeRequestRepository();
            this._employeeRepository = new EmployeeRepository();
            this._emailService = new EmailService();
        }

        /// <summary>
        /// Get overtime requests.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Overtime Request.</returns>
        public IList<OvertimeRequest> GetOvertimeRequests(GetOvertimeMonthlyByEmployeeIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<OvertimeRequest> overtimeRequests = null;

            if (criteria.IsValid())
            {
                IEnumerable<OvertimeRequest> rawOvertimeRequests = GetData<OvertimeRequest>(criteria);

                if (rawOvertimeRequests != null)
                {
                    overtimeRequests = rawOvertimeRequests.ToList();
                }
            }

            return overtimeRequests;
        }

        /// <summary>
        /// Update overtime request.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult UpdateOvertimeRequest(UpdateOvertimeRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Get overtime report in year.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Request Entity.</returns>
        public IList<RequestEntity> GetOvertimeReportInYear(GetOvertimeReportInYearCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> requestEntities = null;

            if (criteria.IsValid())
            {
                requestEntities = _overtimeRepository.GetOvertimeReportInYear(criteria);
            }

            return requestEntities;
        }

        /// <summary>
        /// Get overtime record.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list overtime record.</returns>
        public IList<OvertimeExcelReport> OvertimeRecord(GetOvertimeRecordCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<OvertimeExcelReport> overtimeRecord = null;

            if (criteria.IsValid())
            {
                overtimeRecord = _overtimeRepository.OvertimeRecord(criteria);
            }

            return overtimeRecord;
        }

        /// <summary>
        /// Get yearly overtime report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Overtime Annual Report.</returns>
        public IList<OvertimeAnnualReport> GetYearlyOvertimeReport(GetYearlyOvertimeReportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                //// Role HR
                criteria.Role = "6";
                IEnumerable<OvertimeAnnualReport> rawOvertimeReport = GetData<OvertimeAnnualReport>(criteria);

                if (rawOvertimeReport != null)
                {
                    return rawOvertimeReport.ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get yearly overtime report by normal calendar.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Overtime Annual Report.</returns>
        public IList<OvertimeAnnualReport> GetYearlyOvertimeReportByNormalCalendar(GetYearlyOvertimeReportByNormalCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                //// Role HR
                criteria.Role = "6";
                IEnumerable<OvertimeAnnualReport> overtimeReport = GetData<OvertimeAnnualReport>(criteria);

                if (overtimeReport != null)
                {
                    return overtimeReport.ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Get monthly overtime report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Request Entity.</returns>
        public IList<RequestEntity> GetMonthlyOvertimeReport(GetMemberOvertimeMonthlyCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> overtimeReports = new List<RequestEntity>();

            if (criteria.IsValid())
            {
                overtimeReports = _overtimeRepository.GetMonthlyOvertimeReport(criteria);
            }

            return overtimeReports;
        }

        /// <summary>
        /// Get monthly overtime report by normal calendar.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Request Entity.</returns>
        public IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(GetMonthlyOvertimeReportByNormalCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> overtimeReports = new List<RequestEntity>();

            if (criteria.IsValid())
            {
                overtimeReports = _overtimeRepository.GetMonthlyOvertimeReportByNormalCalendar(criteria);
            }

            return overtimeReports;
        }

        /// <summary>
        /// Get overtime by request Id.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Request Entity.</returns>
        public RequestEntity GetOvertimeRequestById(GetOvertimeByRequestIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            RequestEntity requestEntity = null;

            if (criteria.IsValid())
            {
                return _overtimeRepository.GetOvertimeRequestById(criteria);
            }

            return requestEntity;
        }

        /// <summary>
        /// Get overtime request by HR.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Overtime Request.</returns>
        public IList<OvertimeRequest> HRGetOvertimeRequest(GetOvertimeMonthlyByHRCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<OvertimeRequest> overtimeRequests = null;

            if (criteria.IsValid())
            {
                IEnumerable<OvertimeRequest> rawOvertimeRequest = GetData<OvertimeRequest>(criteria);

                if (rawOvertimeRequest != null)
                {
                    overtimeRequests = rawOvertimeRequest.ToList();
                }
            }

            return overtimeRequests;
        }

        /// <summary>
        /// Get overtime request normal daily by HR.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Overtime Request.</returns>
        public IList<OvertimeRequest> HRGetNormalOvertimeRequest(GetOvertimeNormalDailyByHRCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<OvertimeRequest> overtimeRequests = null;

            if (criteria.IsValid())
            {
                IEnumerable<OvertimeRequest> rawOvertimeRequest = GetData<OvertimeRequest>(criteria);

                if (rawOvertimeRequest != null)
                {
                    overtimeRequests = rawOvertimeRequest.ToList();
                }
            }

            return overtimeRequests;
        }

        /// <summary>
        /// Get member overtime monthly.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Request Entity.</returns>
        public IList<RequestEntity> GetMemberOvertimeMonthly(GetMemberOvertimeMonthlyCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> memberOvertime = null;

            if (criteria.IsValid())
            {
                memberOvertime = _overtimeRepository.GetMemberOvertimeMonthly(criteria);
            }

            return memberOvertime;
        }

        /// <summary>
        /// Function to send mail to reminder claim OT.
        /// </summary>
        /// <param name="subject">The subject mail.</param>
        /// <param name="content">The content mail.</param>
        public void SendMailToReminderClaimOvertime(string subject, string content)
        {
            var employees = _employeeRepository.GetEmployeeCanClaimOvertime();
            if (employees != null)
            {
                List<string> emailList = employees.Where(t => !string.IsNullOrWhiteSpace(t.Email)).Select(t => t.Email).ToList();
                var emails = SplitList(emailList, 100);

                foreach (List<string> list in emails)
                {
                    var mailTo = string.Join(";", list);
                    this._emailService.SendEmail(ConfigSettings.EmailSetting, mailTo, string.Empty, subject, content);
                }
            }
        }

        /// <summary>
        /// Get Overtime excel report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Overtime Request.</returns>
        public IList<OvertimeRequest> OTExcelReportDetails(GetOvertimeExcelReportDetailCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                IEnumerable<OvertimeRequest> reportDetails = GetData<OvertimeRequest>(criteria);

                if (reportDetails != null)
                {
                    return reportDetails.ToList();
                }
            }

            return null;
        }

        /// <summary>
        /// Function to check invalid overtimes.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Result of check.</returns>
        public bool CheckInvalidOvertimes(GetDetailOvertimeByRequestIdsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var isInvalid = false;

            if (criteria.IsValid())
            {
                IEnumerable<OvertimeRequest> rawOvertime = GetData<OvertimeRequest>(criteria);

                if (rawOvertime != null)
                {
                    IList<OvertimeRequest> overtimes = rawOvertime.ToList();

                    if (overtimes != null && overtimes.Any())
                    {
                        var index = 0;
                        while (!isInvalid && index < overtimes.Count())
                        {
                            isInvalid = CheckInvalidOvertimes(overtimes[index]);
                            index++;
                        }
                    }
                }
            }

            return isInvalid;
        }

        /// <summary>
        /// Function to check invalid overtime.
        /// </summary>
        /// <param name="overtime">Overtime Request.</param>
        /// <returns>Result of check.</returns>
        private bool CheckInvalidOvertimes(OvertimeRequest overtime)
        {
            var isInvalid = false;
            if ((!overtime.IsHoliday && !overtime.IsOffDay) || overtime.IsKeepInView)
            {
                if (overtime.OffHours != 0 || overtime.PublicHours != 0 || overtime.TotalWorkHours < (overtime.TotalShiftHours + overtime.NormalHours))
                {
                    isInvalid = true;
                }
            }
            else if (overtime.IsOffDay)
            {
                if (overtime.NormalHours != 0 || overtime.PublicHours != 0 || overtime.TotalWorkHours < overtime.OffHours)
                {
                    isInvalid = true;
                }
            }
            else if (overtime.IsHoliday)
            {
                if (overtime.NormalHours != 0 || overtime.OffHours != 0 || overtime.TotalWorkHours < overtime.PublicHours)
                {
                    isInvalid = true;
                }
            }

            return isInvalid;
        }

        private List<List<string>> SplitList(List<string> locations, int size)
        {
            List<List<string>> emails = new List<List<string>>();

            for (int i = 0; i < locations.Count; i += size)
            {
                emails.Add(locations.GetRange(i, Math.Min(size, locations.Count - i)));
            }

            return emails;
        } 
    }
}
