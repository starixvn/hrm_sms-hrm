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
    using GO.Website.Models.Todoist;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class HomeController : BaseController
    {
        private readonly List<SiteSetting> siteSettings;
        private readonly IHttpClientFactory clientFactory;
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly IBaseService baseService;

        public HomeController(
            List<SiteSetting> siteSettings,
            IHttpClientFactory clientFactory,
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            IAuthService authService,
            IUserService userService,
            IBaseService baseService)
        {
            this.siteSettings = siteSettings;
            this.clientFactory = clientFactory;
            this.userControlSettings = userControlSettings;
            this.userControlSession = userControlSession;
            this.authService = authService;
            this.baseService = baseService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetSites()
        {
            var userId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
            var onUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
            var criteria = new GetUserSiteByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            var usersites = await baseService.GetDataAsync<UserSite>(criteria);
            usersites = usersites.Where(x => x.SiteId != userControlSettings.SiteId).ToList();
            foreach (var usersite in usersites)
            {
                var username = !string.IsNullOrWhiteSpace(usersite.Username) ? usersite.Username : usersite.Code;
                username = !string.IsNullOrWhiteSpace(username) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(username)) : string.Empty;
                usersite.SiteURL = $"{usersite.SSOUrl}?userId={userId}&onUserId={onUserId}&username={username}";
            }

            return Json(usersites);
        }

        [Route("sso/gettime")]
        public async Task<IActionResult> GetTime()
        {
            var time = HttpUtility.UrlEncode(Cryptography.EncryptPassword(await authService.SSOGetTimeAsync()));
            return Json(time);
        }

        public IActionResult GetHrmTodoist()
        {
            var items = new List<TodoistItem>();
            items.Add(new TodoistItem
            {
                SiteId = 1,
                Name = "You have a Leave Request of Jimmy on 24 Mar, 2023",
                TypeId = 1,
                Actions = new List<TodoistAction>
                {
                    new TodoistAction
                    {
                        Name = "Verify",
                        Url = "/api/request/process",
                        Method = "Post",
                        Metadata = new TodoistMetadata
                        {
                            RequestId = 1,
                            EmpId = 1,
                            Code = "00329",
                            Nickname = "Jacky",
                            FullName = "Đặng Vĩ Cường",
                            RequestTypeId = 1,
                            Remark = string.Empty
                        }
                    },
                    new TodoistAction
                    {
                        Name = "Reject",
                        Url = "/api/request/process",
                        Method = "Post",
                        Metadata = new TodoistMetadata
                        {
                            RequestId = 1,
                            EmpId = 1,
                            Code = "00329",
                            Nickname = "Jacky",
                            FullName = "Đặng Vĩ Cường",
                            RequestTypeId = 1,
                            Remark = string.Empty
                        }
                    }
                }
            });
            items.Add(new TodoistItem
            {
                SiteId = 1,
                Name = "You have a Check-in Request of Jimmy on 27 Mar, 2023",
                TypeId = 1,
                Actions = new List<TodoistAction>
                {
                    new TodoistAction
                    {
                        Name = "Approve",
                        Url = "/api/request/process",
                        Method = "Post",
                        Metadata = new TodoistMetadata
                        {
                            RequestId = 2,
                            EmpId = 1,
                            Code = "00329",
                            Nickname = "Jacky",
                            FullName = "Đặng Vĩ Cường",
                            RequestTypeId = 2,
                            Remark = string.Empty,
                            Disciplines = new List<Discipline>
                            {
                                new Discipline
                                {
                                    TypeId = 1,
                                    Name = "Late in",
                                    Details = "Late in 10'"
                                }
                            }
                        }
                    }
                }
            });
            return Json(items);
        }

        public async Task<IActionResult> ProcessAction(short siteId, TodoistAction action)
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
                if(usersite != null)
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
                if(siteSetting != null)
                {
                    token = await GetTokenAsync(siteSetting);
                }

                switch (action.Method.ToLowerInvariant())
                {
                    case "post":
                        await PostAsync<TodoistMetadata, object>(action.Url, action.Metadata, token);
                        break;

                    default:
                        await GetAsync<object>(action.Url, token);
                        break;
                }
            }

            return Ok();
        }

        public IActionResult GetUserInfo()
        {
            User user = userService.GetById(userControlSession.UserId);
            user.UserId = 0;
            user.Password = string.Empty;
            return Json(user);
        }

        private async Task<string> GetTokenAsync(SiteSetting siteSetting)
        {
            using (var request = CreateRequest(siteSetting.TokenUrl, "POST"))
            {
                var client = clientFactory.CreateClient();
                using (var response = await client.SendAsync(request))
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();

                    return jsonResult;
                }
            }
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

        private async Task<T> GetAsync<T>(string requestUrl, string token = null)
        {
            var client = clientFactory.CreateClient();
            using (var request = CreateRequest(requestUrl, "get"))
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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