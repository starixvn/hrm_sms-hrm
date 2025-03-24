namespace HRM.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using HRM.AutoJobs.Criteria;
    using HRM.AutoJobs.Models;
    using HRM.AutoJobs.XMLExtensions;
    using Newtonsoft.Json;

    public class TimeRecordService(
        AppSettings appSettings,
        IHttpClientFactory clientFactory,
        IBaseService baseService,
        ILogger logger) : ITimeRecordService
    {
        public async Task ImportTimeRecordAsync()
        {
            var command = new ImportTimeRecordCommand();
            await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task SyncTimeRecordAsync()
        {
            var toDate = DateTimeOffset.Now.DateTime;
            var fromDate = toDate.AddDays(-2).Date;
            var url = new Uri($"{appSettings.SyncTimeRecordUrl}?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd mm:HH:ss}");
            var httpClient = clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", appSettings.SyncTimeRecordApiKey);
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var rawData = await response.Content.ReadAsStringAsync();
                var records = JsonConvert.DeserializeObject<IList<TimeRecord>>(rawData);
                if (records != null && records.Any())
                {
                    var command = new SyncTimeRecordCommand
                    {
                        FromDate = fromDate,
                        ToDate = toDate,
                        TimeRecordXml = records.ToXML().ToString()
                    };
                    await baseService.ExecuteNonQueryAsync(command);
                }
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                await logger.ErrorAsync($"Sync time record, StatusCode: {response.StatusCode}\r\nMessage: {message}");
            }
        }
    }
}