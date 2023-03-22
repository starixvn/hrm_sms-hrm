namespace GO.Admin.Website.Controllers
{
    using System;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using GO.Admin.Website.Constants;
    using GO.Admin.Website.Criteria.Site;
    using GO.Admin.Website.Filters;
    using GO.Admin.Website.Models.Site;
    using GO.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [UCPermission(FunctionKey = FunctionKey.SiteManagement, Policy = Policy.Read)]
    public class SiteController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;

        public SiteController(
            IUserControlSession userControlSession,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.SiteManagement, Policy.Host);
            return View(isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] GetAllSiteCriteria criteria)
        {
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        public async Task<IActionResult> GetById(short id)
        {
            var criteria = new GetSiteByIdCriteria { SiteId = id };
            var site = await baseService.GetDataEntityAsync<Site>(criteria);
            return Json(site);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.SiteManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateSiteCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            command.AccessToken = GenerateAccessToken();
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.SiteManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] UpdateSiteCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.SiteManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateSiteStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.SiteManagement, Policy = Policy.Host)]
        public IActionResult GenerateToken()
        {
            return Json(GenerateAccessToken());
        }

        private static string GenerateAccessToken()
        {
            var apiKey = string.Empty;
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                // 256 bit
                byte[] secretKeyByteArray = new byte[32];

                cryptoProvider.GetBytes(secretKeyByteArray);
                apiKey = Convert.ToBase64String(secretKeyByteArray);
            }

            return apiKey;
        }
    }
}