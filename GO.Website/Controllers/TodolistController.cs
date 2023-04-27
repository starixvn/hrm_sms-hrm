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
    using GO.Website.Constants;
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
        private readonly List<ApiSiteSetting> apiSiteSettings;
        private readonly IHttpClientFactory clientFactory;
        private readonly IUserControlSession userControlSession;
        private readonly IAuthService authService;
        private readonly IBaseService baseService;
        private readonly IEpaTodolistService epaTodolistService;
        private readonly IFeedbackService feedbackService;

        public TodolistController(
            List<ApiSiteSetting> apiSiteSettings,
            IHttpClientFactory clientFactory,
            IUserControlSession userControlSession,
            IAuthService authService,
            IBaseService baseService,
            IEpaTodolistService epaTodolistService,
            IFeedbackService feedbackService)
        {
            this.apiSiteSettings = apiSiteSettings;
            this.clientFactory = clientFactory;
            this.userControlSession = userControlSession;
            this.authService = authService;
            this.baseService = baseService;
            this.epaTodolistService = epaTodolistService;
            this.feedbackService = feedbackService;
        }

        public async Task<IActionResult> GetTodolistBlock()
        {
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var usersites = await baseService.GetDataAsync<UserSite>(criteria);
            var result = new
            {
                hrm = apiSiteSettings.Any(x => x.IsHrmSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                epa = apiSiteSettings.Any(x => x.IsEpaSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                epa360 = apiSiteSettings.Any(x => x.IsEpa360System && usersites.Any(y => y.SiteId == x.SiteId))
            };
            return Json(result);
        }

        public async Task<IActionResult> GetHrmTodolist()
        {
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var usersites = await baseService.GetDataAsync<UserSite>(criteria);
            var hrmsites = apiSiteSettings.Where(x => x.IsHrmSystem && usersites.Any(y => y.SiteId == x.SiteId)).ToList();
            var todoitems = new List<TodolistItem>();
            foreach (var hrm in hrmsites)
            {
                var usersite = usersites.First(x => x.SiteId == hrm.SiteId);
                var token = await GetTokenAsync(hrm);
                var url = string.Format("{0}{1}?userId={2}", hrm.BaseUrl, hrm.TodolistUrl, usersite.UserId);
                var items = await GetAsync<IList<TodolistItem>>(url, token);
                foreach (var item in items)
                {
                    item.SiteId = hrm.SiteId;
                }

                todoitems.AddRange(items);
            }

            return Json(todoitems);
        }

        public async Task<IActionResult> GetEpaTodolist()
        {
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var userSites = await baseService.GetDataAsync<UserSite>(criteria);
            IList<TodolistItem> todoitems = await epaTodolistService.GetByUserIdAsync(userControlSession.UserId, userSites);

            return Json(todoitems);
        }

        public async Task<IActionResult> GetEpa360Todolist()
        {
            var site = apiSiteSettings.FirstOrDefault(x => x.IsEpa360System);
            IList<TodolistItem> todoitems = await feedbackService.GetTodolistByUserIdAsync(userControlSession.UserId);
            if(site != null)
            {
                foreach(var item in todoitems)
                {
                    item.SiteId = site.SiteId;
                }
            }

            return Json(todoitems);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessAction([FromBody] ProcessActionParam param)
        {
            var userId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
            var onUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var usersites = await baseService.GetDataAsync<UserSite>(criteria);
            if (param.Action.IsRedirect)
            {
                var usersite = usersites.FirstOrDefault(x => x.SiteId == param.SiteId);
                if (usersite != null)
                {
                    var username = !string.IsNullOrWhiteSpace(usersite.Username) ? usersite.Username : usersite.Code;
                    username = !string.IsNullOrWhiteSpace(username) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(username)) : string.Empty;
                    var time = HttpUtility.UrlEncode(Cryptography.EncryptPassword(await authService.SSOGetTimeAsync()));
                    var url = $"{usersite.SSOUrl}?userId={userId}&onUserId={onUserId}&username={username}&time={time}&redirectUrl={param.Action.Url}";
                    return Json(url);
                }
            }
            else
            {
                param.Action.Metadata.UserId = userControlSession.UserId;
                var siteSetting = apiSiteSettings.FirstOrDefault(x => x.SiteId == param.SiteId);
                var token = string.Empty;
                if (siteSetting != null)
                {
                    token = await GetTokenAsync(siteSetting);
                }

                switch (param.Action.Method.ToLowerInvariant())
                {
                    case "post":
                        var result = await PostAsync<TodolistMetadata, CommandResult>(param.Action.Url, param.Action.Metadata, token);
                        return Ok(result);

                    default:
                        await GetAsync<object>(param.Action.Url, token);
                        return Ok();
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

        private async Task<string> GetTokenAsync(ApiSiteSetting siteSetting)
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

                var client = clientFactory.CreateClient(HttpClientName.DefaultHttpClient);
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
            var client = clientFactory.CreateClient(HttpClientName.DefaultHttpClient);
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
            var client = clientFactory.CreateClient(HttpClientName.DefaultHttpClient);
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