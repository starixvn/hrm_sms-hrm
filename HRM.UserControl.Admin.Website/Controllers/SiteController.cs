namespace HRM.UserControl.Admin.Website.Controllers
{
    using System;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Site;
    using HRM.UserControl.Admin.Website.Models;
    using HRM.UserControl.Admin.Website.Models.Site;
    using HRM.UserControl.Admin.Website.Resources;
    using HRM.UserControl.Admin.Website.Services;
    using Microsoft.AspNetCore.Mvc;

    public class SiteController : Controller
    {
        private readonly IBaseService baseService;

        public SiteController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> Create([FromBody] CreateSiteCommand command)
        {
            var commandResult = new CommandResult { Code = 0, Message = string.Empty };
            if (!Uri.IsWellFormedUriString(command.SiteURL, UriKind.Absolute))
            {
                commandResult.Code = 1;
                commandResult.Message = ViewResources.InvalidUrl;

                return Json(commandResult);
            }

            command.CreatedBy = 4;
            command.AccessToken = GenerateAccessToken();
            commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateSiteCommand command)
        {
            var commandResult = new CommandResult { Code = 0, Message = string.Empty };
            if (!Uri.IsWellFormedUriString(command.SiteURL, UriKind.Absolute))
            {
                commandResult.Code = 1;
                commandResult.Message = ViewResources.InvalidUrl;

                return Json(commandResult);
            }

            command.ModifiedBy = 4;
            commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateSiteStatusCommand command)
        {
            command.ModifiedBy = 4;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
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