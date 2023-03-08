namespace GO.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using GO.AutoJobs.Criteria.UnitHead;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Models.UnitHead;
    using HRM.UserControl.Core.Extensions.XML;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class NexHrmUnitHeadService : INexHrmUnitHeadService
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IBaseService baseService;
        private readonly NexHrmApiConfig apiConfig;

        public NexHrmUnitHeadService(
            ILogger<NexHrmUnitHeadService> logger,
            IHttpClientFactory clientFactory,
            IBaseService baseService,
            NexHrmApiConfig apiConfig)
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
                foreach(var info in infos)
                {
                    info.SiteId = apiConfig.SiteId;
                }

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
                foreach (var schedule in schedules)
                {
                    schedule.SiteId = apiConfig.SiteId;
                }

                var command = new UpdateUnitHeadScheduleCommand
                {
                    Value = schedules.ToXML().ToString()
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        private async Task<string> GetTokenAsync()
        {
            try
            {
                var httpClient = clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var authUrl = new Uri($"{apiConfig.BaseUrl}{apiConfig.AuthenticateUrl}" +
                    $"?username={apiConfig.AppCredentialId}&password={apiConfig.AppCredentialPassword}");
                var request = new HttpRequestMessage(HttpMethod.Post, authUrl);
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();

                        return responseData;
                    }
                    else
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        logger.LogError(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));

                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                return default;
            }
        }

        private async Task<T> GetDataFromApiAsync<T>(string url)
        {
            string token = await GetTokenAsync();

            var httpClient = clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

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
                        logger.LogError(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));

                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                return default;
            }
        }
    }
}