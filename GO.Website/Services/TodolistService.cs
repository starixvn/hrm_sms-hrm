namespace GO.Website.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria;
    using GO.Website.Models;
    using GO.Website.Models.Todolist;
    using Newtonsoft.Json;

    public class TodolistService : ITodolistService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger logger;
        private readonly IBaseService baseService;

        public TodolistService(
            IHttpClientFactory clientFactory,
            ILogger logger,
            IBaseService baseService)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.baseService = baseService;
        }

        public async Task<IEnumerable<UserSite>> GetUserSiteByUserIdAsync(int userId)
        {
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userId
            };
            return await baseService.GetDataAsync<UserSite>(criteria);
        }

        public async Task<string> GetTokenAsync(ApiSiteSetting apiSiteSetting)
        {
            var url = string.Format("{0}{1}", apiSiteSetting.BaseUrl, apiSiteSetting.TokenUrl);
            var data = new Dictionary<string, string>
            {
                { "username", apiSiteSetting.Username },
                { "password", apiSiteSetting.Password }
            };

            if (apiSiteSetting.IsHrmSystem)
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                var token = await PostAsync<string>(url, content);
                return token;
            }

            if (apiSiteSetting.IsEpaSystem)
            {
                data.Add("grant_type", "password");
                var content = new FormUrlEncodedContent(data);
                var token = await PostAsync<EpaToken>(url, content);
                return token.Value;
            }

            if (apiSiteSetting.IsHrppSystem)
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                var token = await PostAsync<HrppToken>(url, content);
                return token.Token;
            }

            return string.Empty;
        }

        public async Task<IList<TodolistItem>> GetTodolistAsync(IList<UserSite> usersites, IList<ApiSiteSetting> apiSiteSettings)
        {
            var todoitems = new List<TodolistItem>();
            foreach (ApiSiteSetting apiSiteSetting in apiSiteSettings)
            {
                string token = await GetTokenAsync(apiSiteSetting);
                UserSite usersite = usersites.First(x => x.SiteId == apiSiteSetting.SiteId);
                var username = !string.IsNullOrWhiteSpace(usersite.Username) ? usersite.Username : usersite.Code;
                var url = string.Format("{0}{1}?userId={2}&username={3}", apiSiteSetting.BaseUrl, apiSiteSetting.TodolistUrl, usersite.UserId, username);
                var items = await GetAsync<IList<TodolistItem>>(url, token);
                foreach (var item in items)
                {
                    item.SiteId = usersite.SiteId;
                    item.SiteName = usersite.SiteName;
                }

                todoitems.AddRange(items);
            }

            return todoitems;
        }

        public async Task<T> GetAsync<T>(string requestUrl, string token = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUrl)))
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var client = clientFactory.CreateClient(HttpClientName.DefaultHttpClient);
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
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
        }

        public async Task<T> PostAsync<T>(string requestUrl, HttpContent content = null, string token = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUrl)))
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                if (content != null)
                {
                    request.Content = content;
                }

                var client = clientFactory.CreateClient(HttpClientName.DefaultHttpClient);
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResult = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(jsonResult);
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
        }
    }
}