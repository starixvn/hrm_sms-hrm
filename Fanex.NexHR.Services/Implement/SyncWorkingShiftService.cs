namespace Fanex.NexHR.Services.Implement
{
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISyncWorkingShiftService
    {
        Task SyncWorkingShift();
    }
    public class SyncWorkingShiftService : ISyncWorkingShiftService
    {
        private const string ModifiedDateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";
        private readonly IBaseService baseService;
        private readonly SyncByApiSetting apiSettings;
        private readonly IRestClient restClient;
        public SyncWorkingShiftService(IBaseService baseService, SyncByApiSetting apiSettings)
        {
            this.baseService = baseService;
            restClient = new RestClient { Encoding = Encoding.UTF8 };
            this.apiSettings = apiSettings;
        }
        public async Task SyncWorkingShift()
        {
            var criteria = new GetWorkingShiftLastSyncDateCriteria { };
            var lastModifiedDate = await baseService.GetDataEntityAsync<DateTime?>(criteria);
            var workingShifts = await GetDataFromApi<IEnumerable<WorkingShiftSyncModel>>(apiSettings.SyncShiftUrl, lastModifiedDate);
            if (workingShifts != null && workingShifts.Any())
            {
                var command = new ImportWorkingShiftCommand
                {
                    WorkingShiftXml = workingShifts.ToXML().ToString()
                };
                baseService.ExecuteNonQuery(command);
            }
        }

        protected internal virtual async Task<T> GetDataFromApi<T>(string url, DateTime? lastModifiedDate = null)
        {
            string token = await GetTokenAsync();

            restClient.BaseUrl = new Uri(apiSettings.BaseUrl);
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");

            if (lastModifiedDate != null)
            {
                request.AddParameter("lastModifiedDate", lastModifiedDate.Value.ToString(ModifiedDateTimeFormat));
            }

            var response = await restClient.ExecuteGetAsync<T>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            return default(T);
        }

        private async Task<string> GetTokenAsync()
        {
            restClient.BaseUrl = new Uri(apiSettings.BaseUrl);
            string authUrl = $"{apiSettings.AuthenticateUrl}?username={apiSettings.AppCredentialId}&password={apiSettings.AppCredentialPassword}";
            var request = new RestRequest(authUrl, Method.POST);
            request.AddHeader("Content-type", "application/x-www-form-urlencoded");
            var response = await restClient.ExecutePostAsync<string>(request);
            return response.Data;
        }
    }
}
