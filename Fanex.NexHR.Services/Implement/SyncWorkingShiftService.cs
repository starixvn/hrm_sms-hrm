namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.ScheduleArrange;
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using Newtonsoft.Json;

    public interface ISyncWorkingShiftService
    {
        Task SyncWorkingShiftAsync();

        Task<CommandResult> SyncWorkingShiftAsync(SyncWorkingShiftCommand command);
    }

    public class SyncWorkingShiftService : ISyncWorkingShiftService
    {
        private const int BatchSize = 1000;
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SyncWorkingShiftService));
        private readonly IBaseService baseService;
        private readonly HttpClient _httpClient;

        public SyncWorkingShiftService(IBaseService baseService, HttpClient httpClient)
        {
            this.baseService = baseService;
            _httpClient = httpClient;
        }

        public async Task SyncWorkingShiftAsync()
        {
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime>(new GetLastModifiedDateCriteria());
            var url = new Uri($"{ConfigSettings.SyncWorkingShiftUrl}/{lastModifiedDate:yyyy-MM-dd}/{DateTime.Now:yyyy-MM-dd}");
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var rawData = await response.Content.ReadAsStringAsync();
                var workingShifts = JsonConvert.DeserializeObject<IList<WorkingShiftSyncModel>>(rawData);
                workingShifts = workingShifts?.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeID)
                    && (!x.LastUpdated.HasValue || x.LastUpdated.Value > lastModifiedDate)).ToList();
                if (workingShifts != null && workingShifts.Any())
                {
                    foreach (var shift in workingShifts)
                    {
                        ProcessData(shift);
                    }

                    int totalBatch = (int)Math.Ceiling((double)workingShifts.Count() / BatchSize);
                    for (int i = 0; i < totalBatch; i++)
                    {
                        var command = new ImportWorkingShiftCommand
                        {
                            WorkingShiftXml = workingShifts.Skip(i * BatchSize).Take(BatchSize).ToXML().ToString()
                        };
                        baseService.ExecuteNonQuery(command);
                    }
                }
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                logger.Error(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));
            }
        }

        public async Task<CommandResult> SyncWorkingShiftAsync(SyncWorkingShiftCommand command)
        {
            var commandResult = new CommandResult { Code = 0, Message = string.Empty };
            var url = new Uri($"{ConfigSettings.SyncWorkingShiftUrl}/{command.FromDate:yyyy-MM-dd}/{command.ToDate:yyyy-MM-dd}");
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var rawData = await response.Content.ReadAsStringAsync();
                var workingShifts = JsonConvert.DeserializeObject<IList<WorkingShiftSyncModel>>(rawData);
                workingShifts = workingShifts?.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeID)).ToList();
                if (workingShifts != null && workingShifts.Any())
                {
                    foreach (var shift in workingShifts)
                    {
                        ProcessData(shift);
                    }

                    int totalBatch = (int)Math.Ceiling((double)workingShifts.Count() / BatchSize);
                    for (int i = 0; i < totalBatch; i++)
                    {
                        command.WorkingShiftXml = workingShifts.Skip(i * BatchSize).Take(BatchSize).ToXML().ToString();
                        commandResult = baseService.ExecuteNonQuery(command);
                    }
                }
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                logger.Error(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));
                commandResult.Code = (int)response.StatusCode;
                commandResult.Message = message;
            }

            return commandResult;
        }

        private static void ProcessData(WorkingShiftSyncModel shift)
        {
            shift.EmployeeID = shift.EmployeeID.Trim();
            shift.ShiftCode = shift.ShiftCode?.Trim();

            CheckOffDay(shift);
            CheckPublicHoliday(shift);

            if (shift.ShiftStartTime.HasValue)
            {
                shift.CheckInTime = shift.ShiftStartTime.Value.ToString("HH:mm");
                if (shift.ShiftStartTime.Value.Date > shift.ShiftDate.Date)
                {
                    shift.IsInAfterDay = true;
                }
            }

            if (shift.ShiftEndTime.HasValue)
            {
                shift.CheckOutTime = shift.ShiftEndTime.Value.ToString("HH:mm");
                if (shift.ShiftEndTime.Value.Date > shift.ShiftDate.Date)
                {
                    shift.IsOutAfterDay = true;
                }
            }

            GetTotalHours(shift);
        }

        private static void CheckOffDay(WorkingShiftSyncModel shift)
        {
            if (shift.ShiftCode?.IndexOf("OFF", StringComparison.OrdinalIgnoreCase) >= 0
                && !shift.IsOffDay)
            {
                shift.IsOffDay = true;
            }
        }

        private static void CheckPublicHoliday(WorkingShiftSyncModel shift)
        {
            if (!string.IsNullOrWhiteSpace(shift.ShiftCode)
                && shift.ShiftCode.Equals("PH", StringComparison.OrdinalIgnoreCase)
                && !shift.IsPublicHoliday)
            {
                shift.IsPublicHoliday = true;
            }
        }

        private static void GetTotalHours(WorkingShiftSyncModel shift)
        {
            if (shift.ShiftStartTime.HasValue && shift.ShiftEndTime.HasValue)
            {
                if (shift.ShiftStartTime.Value.Date != shift.ShiftEndTime.Value.Date)
                {
                    shift.IsCrossDay = true;
                }

                const byte NumberOfDigits = 2;
                shift.TotalHours = Math.Round((shift.ShiftEndTime.Value - shift.ShiftStartTime.Value).TotalHours, NumberOfDigits);
            }
        }
    }
}