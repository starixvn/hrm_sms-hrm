namespace GO.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Data;
    using GO.Website.Criteria;
    using GO.Website.Models;
    using GO.Website.Models.Todolist;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class TodolistController : Controller
    {
        private readonly List<SiteSetting> siteSettings;
        private readonly IHttpClientFactory clientFactory;
        private readonly IUserControlSession userControlSession;
        private readonly IAuthService authService;
        private readonly IBaseService baseService;

        public TodolistController(
            List<SiteSetting> siteSettings,
            IHttpClientFactory clientFactory,
            IUserControlSession userControlSession,
            IAuthService authService,
            IBaseService baseService)
        {
            this.siteSettings = siteSettings;
            this.clientFactory = clientFactory;
            this.userControlSession = userControlSession;
            this.authService = authService;
            this.baseService = baseService;
        }

        public async Task<IActionResult> GetHrmTodolist()
        {
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var usersites = await baseService.GetDataAsync<UserSite>(criteria);
            var hrmsites = siteSettings.Where(x => x.IsHrmSystem && usersites.Any(y => y.SiteId == x.SiteId)).ToList();
            var todoitems = new List<TodolistItem>();
            foreach (var hrm in hrmsites)
            {
                var usersite = usersites.First(x => x.SiteId == hrm.SiteId);
                var token = await GetTokenAsync(hrm);
                var url = string.Format("{0}{1}?userId={2}", hrm.BaseUrl, hrm.TodolistUrl, usersite.UserId);
                var items = await GetAsync<IList<TodolistItem>>(url, token);
                foreach(var item in items)
                {
                    item.SiteId = hrm.SiteId;
                }

                todoitems.AddRange(items);
            }

            return Json(todoitems);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessAction(short siteId, TodolistAction action)
        {
            var userId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
            var onUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var usersites = await baseService.GetDataAsync<UserSite>(criteria);
            if (action.IsRedirect)
            {
                var usersite = usersites.FirstOrDefault(x => x.SiteId == siteId);
                if (usersite != null)
                {
                    var username = !string.IsNullOrWhiteSpace(usersite.Username) ? usersite.Username : usersite.Code;
                    username = !string.IsNullOrWhiteSpace(username) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(username)) : string.Empty;
                    var time = HttpUtility.UrlEncode(Cryptography.EncryptPassword(await authService.SSOGetTimeAsync()));
                    var url = $"{usersite.SSOUrl}?userId={userId}&onUserId={onUserId}&username={username}&time={time}&redirectUrl={action.Url}";
                    return Json(url);
                }
            }
            else
            {
                var siteSetting = siteSettings.FirstOrDefault(x => x.SiteId == siteId);
                var token = string.Empty;
                if (siteSetting != null)
                {
                    token = await GetTokenAsync(siteSetting);
                }

                switch (action.Method.ToLowerInvariant())
                {
                    case "post":
                        await PostAsync<TodolistMetadata, object>(action.Url, action.Metadata, token);
                        break;

                    default:
                        await GetAsync<object>(action.Url, token);
                        break;
                }
            }

            return Ok();
        }

        private static HttpRequestMessage CreateRequest(string requestUrl, string method)
        {
            HttpMethod httpMethod;
            switch (method.ToLowerInvariant())
            {
                case "post":
                    httpMethod = HttpMethod.Post;
                    break;

                default:
                    httpMethod = HttpMethod.Get;
                    break;
            }

            return new HttpRequestMessage(httpMethod, new Uri(requestUrl));
        }

        private async Task<string> GetTokenAsync(SiteSetting siteSetting)
        {
            var url = string.Format("{0}{1}", siteSetting.BaseUrl, siteSetting.TokenUrl);
            using (var request = CreateRequest(url, "POST"))
            {
                var data = new Dictionary<string, string>
                {
                    { "username", siteSetting.Username },
                    { "password", siteSetting.Password }
                };
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;

                var client = clientFactory.CreateClient();
                using (var response = await client.SendAsync(request))
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<string>(jsonResult);
                    return token;
                }
            }
        }

        private async Task<T> GetAsync<T>(string requestUrl, string token = null)
        {
            var client = clientFactory.CreateClient();
            using (var request = CreateRequest(requestUrl, "get"))
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        private async Task<TOutput> PostAsync<TInput, TOutput>(string requestUrl, TInput data, string token = null)
        {
            var client = clientFactory.CreateClient();
            using (var request = CreateRequest(requestUrl, "post"))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }
    }
}