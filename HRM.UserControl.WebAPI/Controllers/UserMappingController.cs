namespace HRM.UserControl.WebAPI.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading.Tasks;
    using HRM.UserControl.Models;
    using HRM.UserControl.WebAPI.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class UserMappingController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserMappingService userMappingService;

        public UserMappingController(
            IHttpContextAccessor httpContextAccessor,
            IUserMappingService userMappingService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userMappingService = userMappingService;
        }

        [HttpGet]
        [Route("api/UserMapping/GetBySiteId")]
        public async Task<IActionResult> GetBySiteId(DateTime? lastModifiedDate)
        {
            SiteModel requestSite = GetSiteFromRequestHeader();
            var userMappings = await userMappingService.GetBySiteIdAsync(requestSite.SiteId, lastModifiedDate);

            return Json(userMappings);
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
