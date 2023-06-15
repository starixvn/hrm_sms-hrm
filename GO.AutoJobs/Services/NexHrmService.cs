namespace GO.AutoJobs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using GO.AutoJobs.Criteria.HRM;
    using GO.AutoJobs.Criteria.UnitHead;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Models.HRM;
    using GO.AutoJobs.Models.UnitHead;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Core.Services;
    using Newtonsoft.Json;

    public class NexHrmService : INexHrmService
    {
        private const string ModifiedDateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";
        private readonly DateTime DefaultLastModifiedDate = new DateTime(2010, 1, 1);
        private readonly ILogger logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IBaseService baseService;
        private readonly NexHrmApiConfig apiConfig;

        public NexHrmService(
            ILogger logger,
            IHttpClientFactory clientFactory,
            IBaseService baseService,
            NexHrmApiConfig apiConfig)
        {
            this.logger = logger;
            this.clientFactory = clientFactory;
            this.baseService = baseService;
            this.apiConfig = apiConfig;
        }

        public async Task<CommandResult> SyncEmployeeInfoAsync(bool isNexcel = true)
        {
            var siteId = isNexcel ? apiConfig.NexHrmSiteId : apiConfig.LumiHrmSiteId;
            var commandResult = new CommandResult { Code = 0 };
            var criteria = new GetEmployeeLastModifiedDateBySiteIdCriteria { SiteId = siteId };
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(criteria);
            var employees = await GetDataFromApiAsync<IEnumerable<Employee>>(apiConfig.EmployeeUrl, isNexcel, lastModifiedDate);
            if (employees != null && employees.Any())
            {
                var command = new ImportEmployeeCommand
                {
                    SiteId = siteId,
                    Value = employees.ToXML().ToString()
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public async Task<CommandResult> SyncTeamAsync(bool isNexcel = true)
        {
            var siteId = isNexcel ? apiConfig.NexHrmSiteId : apiConfig.LumiHrmSiteId;
            var commandResult = new CommandResult { Code = 0 };
            var criteria = new GetTeamLastModifiedDateBySiteIdCriteria { SiteId = siteId };
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(criteria);
            var teams = await GetDataFromApiAsync<IEnumerable<Team>>(apiConfig.TeamUrl, isNexcel, lastModifiedDate ?? DefaultLastModifiedDate);
            if (teams != null && teams.Any())
            {
                var command = new ImportTeamCommand
                {
                    SiteId = siteId,
                    Value = teams.ToXML().ToString()
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public async Task<CommandResult> SyncSectionAsync(bool isNexcel = true)
        {
            var siteId = isNexcel ? apiConfig.NexHrmSiteId : apiConfig.LumiHrmSiteId;
            var commandResult = new CommandResult { Code = 0 };
            var criteria = new GetSectionLastModifiedDateBySiteIdCriteria { SiteId = siteId };
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(criteria);
            var sections = await GetDataFromApiAsync<IEnumerable<Section>>(apiConfig.SectionUrl, isNexcel, lastModifiedDate ?? DefaultLastModifiedDate);
            if (sections != null && sections.Any())
            {
                var command = new ImportSectionCommand
                {
                    SiteId = siteId,
                    Value = sections.ToXML().ToString()
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public async Task<CommandResult> SyncJobTitleAsync(bool isNexcel = true)
        {
            var siteId = isNexcel ? apiConfig.NexHrmSiteId : apiConfig.LumiHrmSiteId;
            var commandResult = new CommandResult { Code = 0 };
            var jobTitles = await GetDataFromApiAsync<IEnumerable<JobTitle>>(apiConfig.JobTitleUrl, isNexcel);
            if (jobTitles != null && jobTitles.Any())
            {
                var command = new ImportJobTitleCommand
                {
                    SiteId = siteId,
                    Value = jobTitles.ToXML().ToString()
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public async Task<CommandResult> SyncUniHeadInfoAsync()
        {
            var commandResult = new CommandResult { Code = 0 };
            var infos = await GetDataFromApiAsync<IEnumerable<UnitHeadInfo>>(apiConfig.UnitHeadInfoUrl);
            if (infos != null && infos.Any())
            {
                foreach(var info in infos)
                {
                    info.SiteId = apiConfig.NexHrmSiteId;
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
                    schedule.SiteId = apiConfig.NexHrmSiteId;
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

        private async Task<T> GetDataFromApiAsync<T>(string url, bool isNexcel = true, DateTime? lastModifiedDate = null)
        {
            string token = await GetTokenAsync();

            var httpClient = clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var requestUrl = new StringBuilder($"{apiConfig.BaseUrl}{url}");
            requestUrl.AppendFormat("?isNexcel={0}", isNexcel);
            requestUrl.AppendFormat("&lastModifiedDate={0}", lastModifiedDate.HasValue ? lastModifiedDate.Value.ToString(ModifiedDateTimeFormat) : string.Empty);

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl.ToString()));
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