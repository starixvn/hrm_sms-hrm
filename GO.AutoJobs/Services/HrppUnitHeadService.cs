namespace GO.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using GO.AutoJobs.Criteria.UnitHead;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Models.UnitHead;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Core.Services;
    using Newtonsoft.Json;

    public class HrppUnitHeadService : IHrppUnitHeadService
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IBaseService baseService;
        private readonly HrppApiConfig apiConfig;

        public HrppUnitHeadService(
            ILogger logger,
            IHttpClientFactory clientFactory,
            IBaseService baseService,
            HrppApiConfig apiConfig)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.baseService = baseService;
            this.apiConfig = apiConfig;
        }

        public async Task<CommandResult> SyncUniHeadInfoAsync()
        {
            var commandResult = new CommandResult { Code = 0 };
            var infos = await GetDataFromApiAsync<IEnumerable<UnitHeadInfo>>(apiConfig.UnitHeadInfoUrl);
            if (infos != null && infos.Any())
            {
                var command = new UpdateUnitHeadInfoCommand
                {
                    Value = infos.ToXML().ToString()
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public async Task<CommandResult> SyncUniHeadScheduleAsync()
        {
            var commandResult = new CommandResult { Code = 0 };
            var schedules = await GetDataFromApiAsync<IEnumerable<UnitHeadDailySchedule>>(apiConfig.UnitHeadScheduleUrl);
            if (schedules != null && schedules.Any())
            {
                var command = new UpdateUnitHeadScheduleCommand
                {
                    Value = schedules.ToXML().ToString()
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        private async Task<T> GetDataFromApiAsync<T>(string url)
        {
            var httpClient = clientFactory.CreateClient();
            var requestUrl = $"{apiConfig.BaseUrl}{url}";
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl));
            try
            {
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }
                    else
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        await logger.ErrorAsync(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));

                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message);

                return default;
            }
        }
    }
}