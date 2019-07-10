namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;

    public class OvertimeRequestService : IOvertimeRequestService
    {
        private readonly IOvertimeRequestRepository overtimeRepository;
        private readonly IBaseService baseService;
        private readonly IEmailService emailService;

        public OvertimeRequestService(
            IOvertimeRequestRepository overtimeRepository,
            IBaseService baseService,
            IEmailService emailService)
        {
            this.overtimeRepository = overtimeRepository;
            this.baseService = baseService;
            this.emailService = emailService;
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
            return baseService.GetData<OvertimeDaily>(criteria).ToList();
        }

        public IEnumerable<MonthlyOvertimeRequest> GetMonthlyRequestByEmployeeId(int employeeId, int year)
        {
            var criteria = new GetMonthlyOvertimeRequestByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                Year = year
            };
            IEnumerable<MonthlyOvertimeRequest> monthlyRequests = overtimeRepository.GetMonthlyRequestByEmployeeId(criteria);

            return monthlyRequests;
        }

        public MonthlyOvertimeRequest GetMonthlyRequestDetailsByEmployeeId(int employeeId, DateTime month)
        {
            var criteria = new GetMonthlyOvertimeRequestDetailsByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                Month = month
            };
            MonthlyOvertimeRequest monthlyRequest = overtimeRepository.GetMonthlyRequestDetailsByEmployeeId(criteria);

            return monthlyRequest ?? new MonthlyOvertimeRequest();
        }

        public IEnumerable<MonthlyOvertimeRequest> GetMemberMonthlyRequest(GetMonthlyOvertimeRequestParam param)
        {
            var criteria = new GetMemberMonthlyOvertimeRequestCriteria
            {
                ActorId = param.ActorId,
                Month = param.Month,
                StatusIds = param.StatusIds,
                GroupIds = param.SectionIds,
                EmployeeId = param.EmployeeId,
                IsProcessor = param.IsProcessor
            };
            IEnumerable<MonthlyOvertimeRequest> memberRequests = overtimeRepository.GetMemberMonthlyRequest(criteria);
            return memberRequests;
        }

        public MonthlyOvertimeRequest GetMonthlyRequestDetailsById(long requestId)
        {
            var criteria = new GetMonthlyOvertimeRequestDetailsByIdCriteria
            {
                RequestId = requestId
            };
            MonthlyOvertimeRequest monthlyRequest = overtimeRepository.GetMonthlyRequestDetailsByEmployeeId(criteria);

            return monthlyRequest ?? new MonthlyOvertimeRequest();
        }

        public IList<OvertimeExcelReport> OvertimeRecord(GetOvertimeRecordCriteria criteria)
        {
            IList<OvertimeExcelReport> overtimeRecord = null;

            if (criteria.IsValid())
            {
                overtimeRecord = overtimeRepository.OvertimeRecord(criteria);
            }

            return overtimeRecord;
        }

        public IList<OvertimeDaily> HRGetOvertimeRequest(GetOvertimeMonthlyByHRCriteria criteria)
        {
            return baseService.GetData<OvertimeDaily>(criteria).ToList();
        }

        public IList<OvertimeDaily> HRGetNormalOvertimeRequest(GetOvertimeNormalDailyByHRCriteria criteria)
        {
            return baseService.GetData<OvertimeDaily>(criteria).ToList();
        }

        public void SendMailToReminderClaimOvertime(string subject, string content)
        {
            var criteria = new GetListEmployeeClaimOvertimeCriteria();
            IList<SupervisorNameModel> employees = baseService.GetData<SupervisorNameModel>(criteria).ToList();

            if (employees != null)
            {
                List<string> emailList = employees.Where(t => !string.IsNullOrWhiteSpace(t.Email)).Select(t => t.Email).ToList();
                var emails = SplitList(emailList, 100);

                foreach (List<string> list in emails)
                {
                    var mailTo = string.Join(";", list);
                    emailService.SendEmail(ConfigSettings.EmailSetting, mailTo, string.Empty, subject, content);
                }
            }
        }

        public IList<OvertimeDaily> OTExcelReportDetails(GetOvertimeExcelReportDetailCriteria criteria)
        {
            return baseService.GetData<OvertimeDaily>(criteria).ToList();
        }

        public bool CheckInvalidOvertimes(string requestIds)
        {
            var isInvalid = false;
            var criteria = new GetDetailOvertimeByRequestIdsCriteria { ListRequestId = requestIds };
            IList<OvertimeDaily> overtimes = baseService.GetData<OvertimeDaily>(criteria).ToList();
            if (overtimes != null && overtimes.Any())
            {
                var index = 0;
                while (!isInvalid && index < overtimes.Count())
                {
                    isInvalid = CheckInvalidOvertimes(overtimes[index]);
                    index++;
                }
            }

            return isInvalid;
        }

        public CommandResult EditMonthlyRequest(int actorId, EditMonthlyOvertimeRequest model)
        {
            var criteria = new EditMonthlyOvertimeRequestCriteria
            {
                ActorId = actorId,
                EmployeeId = model.EmployeeId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                DailyDetailsXml = model.DailyOvertimes.ToXML().ToString()
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditMonthlyRequestStatus(int actorId, string requestIds, string command, byte roleId)
        {
            var criteria = new EditMonthlyOvertimeRequestStatusCriteria
            {
                CommandProcess = command,
                RequestIds = requestIds,
                ActorId = actorId,
                RoleId = roleId
            };
            return baseService.ExecuteNonQuery(criteria);
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