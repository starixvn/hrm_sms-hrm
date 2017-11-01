namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class OvertimeRequestService : BaseService, IOvertimeRequestService
    {
        private readonly IOvertimeRequestRepository overtimeRepository;
        private readonly IEmailService emailService;

        public OvertimeRequestService()
            : this(null, null)
        {
        }

        public OvertimeRequestService(IOvertimeRequestRepository overtimeRepository, IEmailService emailService)
        {
            this.overtimeRepository = overtimeRepository ?? new OvertimeRequestRepository();
            this.emailService = emailService ?? new EmailService();
        }

        public OvertimeRequest GetOvertimeRequestByMonth(GetOvertimeRequestByMonthCriteria criteria)
        {
            OvertimeRequest overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = overtimeRepository.GetOvertimeRequestByMonth(criteria);
            }

            return overtime;
        }

        public OvertimeRequest GetOvertimeRequestById(GetOvertimeRequestByIdCriteria criteria)
        {
            OvertimeRequest overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = overtimeRepository.GetOvertimeRequestById(criteria);
            }

            return overtime;
        }

        public OvertimeRequest GetOvertimeDailyReport(GetOvertimeDailyReportCriteria criteria)
        {
            OvertimeRequest overtime = null;
            if (criteria != null && criteria.IsValid())
            {
                overtime = overtimeRepository.GetOvertimeDailyReport(criteria);
            }

            return overtime;
        }

        public IList<OvertimeDaily> GetOvertimeRequests(GetOvertimeMonthlyByEmployeeIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                IList<OvertimeDaily> overtimeRequests = GetData<OvertimeDaily>(criteria).ToList();
                return overtimeRequests;
            }

            return null;
        }

        public IList<RequestEntity> GetOvertimeReportInYear(GetOvertimeReportInYearCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> requestEntities = null;

            if (criteria.IsValid())
            {
                requestEntities = overtimeRepository.GetOvertimeReportInYear(criteria);
            }

            return requestEntities;
        }

        public IList<OvertimeExcelReport> OvertimeRecord(GetOvertimeRecordCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<OvertimeExcelReport> overtimeRecord = null;

            if (criteria.IsValid())
            {
                overtimeRecord = overtimeRepository.OvertimeRecord(criteria);
            }

            return overtimeRecord;
        }

        public IList<OvertimeAnnualReportViewModel> GetYearlyOvertimeReport(GetYearlyOvertimeReportCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                IList<OvertimeAnnualReportViewModel> overtimeReport = GetData<OvertimeAnnualReportViewModel>(criteria).ToList();
                return overtimeReport;
            }

            return null;
        }

        public IList<OvertimeAnnualReportViewModel> GetYearlyOvertimeReportByNormalCalendar(GetYearlyOvertimeReportByNormalCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                IList<OvertimeAnnualReportViewModel> overtimeReport = GetData<OvertimeAnnualReportViewModel>(criteria).ToList();
                return overtimeReport;
            }

            return null;
        }

        public IList<RequestEntity> GetMonthlyOvertimeReport(GetMemberOvertimeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> overtimeReports = new List<RequestEntity>();

            if (criteria.IsValid())
            {
                overtimeReports = overtimeRepository.GetMonthlyOvertimeReport(criteria);
            }

            return overtimeReports;
        }

        public IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(GetMonthlyOvertimeReportByNormalCalendarCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> overtimeReports = new List<RequestEntity>();

            if (criteria.IsValid())
            {
                overtimeReports = overtimeRepository.GetMonthlyOvertimeReportByNormalCalendar(criteria);
            }

            return overtimeReports;
        }

        public IList<OvertimeDaily> HRGetOvertimeRequest(GetOvertimeMonthlyByHRCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            IList<OvertimeDaily> overtimeRequest;

            overtimeRequest = (criteria.IsValid()) ? GetData<OvertimeDaily>(criteria).ToList() : new List<OvertimeDaily>();

            return overtimeRequest;
        }

        public IList<OvertimeDaily> HRGetNormalOvertimeRequest(GetOvertimeNormalDailyByHRCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            IList<OvertimeDaily> overtimeRequest;

            overtimeRequest = (criteria.IsValid()) ? GetData<OvertimeDaily>(criteria).ToList() : new List<OvertimeDaily>();

            return overtimeRequest;
        }

        public IList<OvertimeRequest> GetMemberOvertime(GetMemberOvertimeCriteria criteria)
        {
            IList<OvertimeRequest> memberOvertimes = null;
            if (criteria != null && criteria.IsValid())
            {
                memberOvertimes = overtimeRepository.GetMemberOvertime(criteria);
            }

            return memberOvertimes ?? new List<OvertimeRequest>();
        }

        public IList<OvertimeRequest> GetMemberOvertimeByLeader(GetMemberOvertimeByLeaderCriteria criteria)
        {
            IList<OvertimeRequest> memberOvertimes = null;
            if (criteria != null && criteria.IsValid())
            {
                memberOvertimes = overtimeRepository.GetMemberOvertime(criteria);
            }

            return memberOvertimes ?? new List<OvertimeRequest>();
        }

        public void SendMailToReminderClaimOvertime(string subject, string content)
        {
            IList<SupervisorNameModel> employees = null;
            var criteria = new GetListEmployeeClaimOvertimeCriteria();
            if (criteria.IsValid())
            {
                employees = this.GetData<SupervisorNameModel>(criteria).ToList();
            }

            if (employees != null)
            {
                List<string> emailList = employees.Where(t => !string.IsNullOrWhiteSpace(t.Email)).Select(t => t.Email).ToList();
                var emails = SplitList(emailList, 100);

                foreach (List<string> list in emails)
                {
                    var mailTo = string.Join(";", list);
                    this.emailService.SendEmail(ConfigSettings.EmailSetting, mailTo, string.Empty, subject, content);
                }
            }
        }

        public IList<OvertimeDaily> OTExcelReportDetails(GetOvertimeExcelReportDetailCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            IList<OvertimeDaily> reportDetails;
            reportDetails = (criteria.IsValid()) ? GetData<OvertimeDaily>(criteria).ToList() : new List<OvertimeDaily>();

            return reportDetails;
        }

        public bool CheckInvalidOvertimes(GetDetailOvertimeByRequestIdsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var isInvalid = false;

            if (criteria.IsValid())
            {
                IEnumerable<OvertimeDaily> rawOvertime = GetData<OvertimeDaily>(criteria);

                if (rawOvertime == null)
                {
                    return isInvalid;
                }

                IList<OvertimeDaily> overtimes = rawOvertime.ToList();

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

            return isInvalid;
        }

        private static bool CheckInvalidOvertimes(OvertimeDaily overtime)
        {
            var isInvalid = false;
            if ((!overtime.IsHoliday && !overtime.IsOffDay) || overtime.IsKeepInView)
            {
                isInvalid |= IsInvalidKeepInView(overtime);
            }

            if (overtime.IsOffDay)
            {
                isInvalid |= IsInvalidOffDay(overtime);
            }

            if (overtime.IsHoliday)
            {
                isInvalid |= IsInvalidHoliday(overtime);
            }

            return isInvalid;
        }

        private static bool IsInvalidKeepInView(OvertimeDaily overtime)
        {
            return (!overtime.OffHours.Equals(0) ||
                !overtime.PublicHours.Equals(0) ||
                overtime.TotalWorkHours < (overtime.TotalShiftHours + overtime.NormalHours));
        }

        private static bool IsInvalidOffDay(OvertimeDaily overtime)
        {
            return (!overtime.NormalHours.Equals(0) ||
                !overtime.PublicHours.Equals(0) ||
                overtime.TotalWorkHours < overtime.OffHours);
        }

        private static bool IsInvalidHoliday(OvertimeDaily overtime)
        {
            return (!overtime.NormalHours.Equals(0) ||
                !overtime.OffHours.Equals(0) ||
                overtime.TotalWorkHours < overtime.PublicHours);
        }

        private static List<List<string>> SplitList(List<string> locations, int size)
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