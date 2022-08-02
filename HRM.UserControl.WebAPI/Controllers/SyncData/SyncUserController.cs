namespace HRM.UserControl.WebAPI.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models;
    using HRM.UserControl.WebAPI.Criteria.SyncData;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class SyncUserController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IBaseService baseService;

        public SyncUserController(
            IHttpContextAccessor httpContextAccessor,
            IBaseService baseService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.baseService = baseService;
        }

        [HttpPost]
        [Route("api/sync-data/user/create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var requestSite = GetSiteFromRequestHeader();
            command.SiteId = requestSite.SiteId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            commandResult.ObjectId = command.UserId;
            return Json(commandResult);
        }

        [HttpPost]
        [Route("api/sync-data/user/update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserInfoCommand command)
        {
            var requestSite = GetSiteFromRequestHeader();
            command.SiteId = requestSite.SiteId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            commandResult.ObjectId = command.UserId;
            return Json(commandResult);
        }

        private SiteModel GetSiteFromRequestHeader()
        {
            string jwtToken = httpContextAccessor.HttpContext.Request.Headers["SiteIdentity"].ToString();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken.Replace("Bearer ", string.Empty));
            var siteJson = token.Payload.SerializeToJson();
            return JsonConvert.DeserializeObject<SiteModel>(siteJson);
        }
    }
}