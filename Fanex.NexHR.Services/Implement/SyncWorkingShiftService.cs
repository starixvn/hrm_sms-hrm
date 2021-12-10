namespace Fanex.NexHR.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using Newtonsoft.Json;

    public interface ISyncWorkingShiftService
    {
        Task SyncWorkingShiftAsync();
    }

    public class SyncWorkingShiftService : ISyncWorkingShiftService
    {
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
                workingShifts = workingShifts?.Where(x => !string.IsNullOrWhiteSpace(x.EmployeeID) && x.LastUpdated > lastModifiedDate).ToList();
                if (workingShifts != null && workingShifts.Any())
                {
                    foreach (var shift in workingShifts)
                    {
                        ProcessData(shift);
                    }

                    var command = new ImportWorkingShiftCommand
                    {
                        WorkingShiftXml = workingShifts.ToXML().ToString()
                    };
                    baseService.ExecuteNonQuery(command);
                }
            }
        }

        public static void ProcessData(WorkingShiftSyncModel shift)
        {
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

            if (shift.ShiftStartTime.HasValue && shift.ShiftEndTime.HasValue)
            {
                if(shift.ShiftStartTime.Value.Date != shift.ShiftEndTime.Value.Date)
                {
                    shift.IsCrossDay = true;
                }

                const byte NumberOfDigits = 2;
                shift.TotalHours = Math.Round((shift.ShiftEndTime.Value - shift.ShiftStartTime.Value).TotalHours, NumberOfDigits);
            }
        }
    }
}