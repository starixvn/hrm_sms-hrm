namespace Fanex.NexHR.Services.Implement
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public interface ISyncWorkingShiftService
    {
        Task SyncWorkingShift();
    }
    public class SyncWorkingShiftService : ISyncWorkingShiftService
    {
        private readonly IBaseService baseService;
        public SyncWorkingShiftService(IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task SyncWorkingShift()
        {
            
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigSettings.SyncWorkingShiftBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var criteria = new GetWorkingShiftLastSyncDateCriteria { };
                DateTime lastSyncDate = await baseService.GetDataEntityAsync<DateTime>(criteria);
                var SyncUrl = $"SyncWorkingShift/{lastSyncDate.ToString("yyyy-MM-dd")}/{DateTime.Now.ToString("yyyy-MM-dd")}";
                var response = await client.GetAsync(SyncUrl);
                if (response.IsSuccessStatusCode)
                {
                    var workingShifts = (await response.Content.ReadAsAsync<IEnumerable<WorkingShiftSyncModel>>()).ToList();
                    if (workingShifts != null && workingShifts.Any())
                    {
                        var i = 0;
                        foreach (var shift in workingShifts)
                        {
                            if (i == 0)
                            {
                                shift.EmployeeID = "00319";
                                shift.ShiftCode = "WFH";
                            }
                            if (i == 1)
                            {
                                shift.EmployeeID = "00407";
                            }
                            ProcessSyncData(shift);
                            i++;
                        }
                        var command = new ImportWorkingShiftCommand
                        {
                            WorkingShiftXml = workingShifts.ToXML().ToString()
                        };
                        baseService.GetData<WorkingShiftSyncModel>(command);
                    }
                }
            }
            
        }

        public static void ProcessSyncData(WorkingShiftSyncModel shift)
        {
            var shiftDate = DateTime.Parse(shift.ShiftDate);
            if (shift.ShiftStartTime != null)
            {
                var fromDate = DateTime.Parse(shift.ShiftStartTime);
                shift.CheckInTime = fromDate.ToString("HH:mm");
                if (fromDate.Date > shiftDate.Date)
                {
                    shift.IsInAfterDay = 1;
                }
            }

            if (shift.ShiftEndTime != null)
            {
                var toDate = DateTime.Parse(shift.ShiftEndTime);
                shift.CheckOutTime = toDate.ToString("HH:mm");
                if (toDate.Date > shiftDate.Date)
                {
                    shift.IsOutAfterDay = 1;
                }
            }
            if (shift.IsInAfterDay == 0 && shift.IsOutAfterDay == 1)
            {
                shift.IsCrossDay = 1;
            }
            if (shift.ShiftStartTime != null && shift.ShiftEndTime != null)
            {
                shift.TotalHours = Math.Round((DateTime.Parse(shift.ShiftEndTime) - DateTime.Parse(shift.ShiftStartTime)).TotalHours, 2);
            }
        }
    }
   
}
