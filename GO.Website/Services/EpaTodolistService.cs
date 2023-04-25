namespace GO.Website.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using GO.Website.Constants;
    using GO.Website.Models;
    using GO.Website.Models.Todolist;
    using Newtonsoft.Json;

    public class EpaTodolistService : IEpaTodolistService
    {
        private readonly List<ApiSiteSetting> apiSiteSettings;
        private readonly IHttpClientFactory clientFactory;

        public EpaTodolistService(
            List<ApiSiteSetting> apiSiteSettings,
            IHttpClientFactory clientFactory)
        {
            this.apiSiteSettings = apiSiteSettings;
            this.clientFactory = clientFactory;
        }

        public async Task<IList<TodolistItem>> GetByUserIdAsync(int userId, IEnumerable<UserSite> userSites)
        {
            var epas = apiSiteSettings.Where(x => x.IsEpaSystem && userSites.Any(y => y.SiteId == x.SiteId)).ToList();
            var todoitems = new List<TodolistItem>();
            foreach (var epa in epas)
            {
                var usersite = userSites.First(x => x.SiteId == epa.SiteId);
                EpaToken token = await GetTokenAsync(epa);
                var url = string.Format("{0}{1}?userId={2}", epa.BaseUrl, epa.TodolistUrl, usersite.UserId);
                var items = await GetTodolistAsync(url, token.Value);
                foreach (var item in items)
                {
                    item.SiteId = usersite.SiteId;
                    item.SiteName = usersite.SiteName;
                }

                todoitems.AddRange(items);
            }

            return todoitems;
        }

        private async Task<EpaToken> GetTokenAsync(ApiSiteSetting siteSetting)
        {
            var url = string.Format("{0}{1}", siteSetting.BaseUrl, siteSetting.TokenUrl);
            var data = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", siteSetting.Username },
                { "password", siteSetting.Password }
            };
            var encodedContent = new FormUrlEncodedContent(data);

            var client = clientFactory.CreateClient(HttpClientName.DefaultHttpClient);
            using (var response = await client.PostAsync(new Uri(url), encodedContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<EpaToken>(responseData);
                    return token;
                }

                return new EpaToken();
            }
        }

        private async Task<IList<TodolistItem>> GetTodolistAsync(string requestUrl, string token = null)
        {
            var client = clientFactory.CreateClient(HttpClientName.DefaultHttpClient);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using (var response = await client.GetAsync(new Uri(requestUrl)))
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<IList<TodolistItem>>(responseData);
                    return items;
                }

                return new List<TodolistItem>();
            }
        }
    }
}