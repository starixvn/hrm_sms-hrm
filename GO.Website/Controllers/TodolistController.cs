namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Data;
    using GO.Core.Models;
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
        private readonly IUserControlSession userControlSession;
        private readonly IAuthService authService;
        private readonly ITodolistService todolistService;
        private readonly IFeedbackService feedbackService;

        public TodolistController(
            List<ApiSiteSetting> apiSiteSettings,
            IUserControlSession userControlSession,
            IAuthService authService,
            ITodolistService todolistService,
            IFeedbackService feedbackService)
        {
            this.apiSiteSettings = apiSiteSettings;
            this.userControlSession = userControlSession;
            this.authService = authService;
            this.todolistService = todolistService;
            this.feedbackService = feedbackService;
        }

        public async Task<IActionResult> GetTodolistBlock()
        {
            IEnumerable<UserSite> usersites = await todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId);
            var result = new
            {
                hrm = apiSiteSettings.Any(x => x.IsHrmSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                hrpp = apiSiteSettings.Any(x => x.IsHrppSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                epa = apiSiteSettings.Any(x => x.IsEpaSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                epa360 = apiSiteSettings.Any(x => x.IsEpa360System && usersites.Any(y => y.SiteId == x.SiteId))
            };
            return Json(result);
        }

        public async Task<IActionResult> GetHrmTodolist()
        {
            IEnumerable<UserSite> usersites = await todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId);
            IList<ApiSiteSetting> hrmsites = apiSiteSettings.Where(x => x.IsHrmSystem && usersites.Any(y => y.SiteId == x.SiteId)).ToList();
            IList<TodolistItem> todoitems = await todolistService.GetTodolistAsync(usersites.ToList(), hrmsites);
            return Json(todoitems);
        }

        public async Task<IActionResult> GetEpaTodolist()
        {
            IEnumerable<UserSite> usersites = await todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId);
            IList<ApiSiteSetting> epas = apiSiteSettings.Where(x => x.IsEpaSystem && usersites.Any(y => y.SiteId == x.SiteId)).ToList();
            IList<TodolistItem> todoitems = await todolistService.GetTodolistAsync(usersites.ToList(), epas);
            return Json(todoitems);
        }

        public async Task<IActionResult> GetHrppTodolist()
        {
            IEnumerable<UserSite> usersites = await todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId);
            IList<ApiSiteSetting> hrpps = apiSiteSettings.Where(x => x.IsHrppSystem && usersites.Any(y => y.SiteId == x.SiteId)).ToList();
            IList<TodolistItem> todoitems = await todolistService.GetTodolistAsync(usersites.ToList(), hrpps);
            return Json(todoitems);
        }

        public async Task<IActionResult> GetEpa360Todolist()
        {
            var site = apiSiteSettings.FirstOrDefault(x => x.IsEpa360System);
            IList<TodolistItem> todoitems = await feedbackService.GetTodolistByUserIdAsync(userControlSession.UserId);
            if (site != null)
            {
                foreach (var item in todoitems)
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
            IEnumerable<UserSite> usersites = await todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId);
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
                    token = await todolistService.GetTokenAsync(siteSetting);
                }

                switch (param.Action.Method.ToLowerInvariant())
                {
                    case "post":
                        var encodedData = JsonConvert.SerializeObject(param.Action.Metadata);
                        var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                        var result = await todolistService.PostAsync<CommandResult>(param.Action.Url, content, token);
                        return Ok(result);

                    default:
                        await todolistService.GetAsync<object>(param.Action.Url, token);
                        return Ok();
                }
            }

            return Ok();
        }
    }
}