namespace HRM.UserControl.WebAPI.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria;
    using HRM.UserControl.API.Core.Services;
    using HRM.UserControl.API.Core.Services.Interfaces;
    using HRM.UserControl.Models;
    using HRM.UserControl.Models.Auth;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class AuthController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;
        private readonly IJwtManagerService jwtManagerService;

        public AuthController(IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IJwtManagerService jwtManagerService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
            this.jwtManagerService = jwtManagerService;
        }

        [Route("api/user/getbyemail")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            User user = await userService.GetUserByEmailAsync(email);

            return Ok(user);
        }

        [Route("api/auth/signin")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginParam loginParams)
        {
            LoginResult loginResult = new();
            User user = await userService.GetUserByIdAsync(loginParams.UserId);

            if (user is null)
            {
                loginResult.ErrorMessage = "User does not exists";
                return Ok(loginResult);
            }
            var currentRequestSite = GetUserSiteFromRequestHeader();
            var userSites = await userService.GetUserSiteCompanyAsync(loginParams.UserId);
            if (!userSites.Any(site => site.SiteId == currentRequestSite.SiteId))
            {
                loginResult.ErrorMessage = "User have not permission on this site";
                return Ok(loginResult);
            }

            loginResult.Token = await jwtManagerService.GenerateToken(user.UserId, currentRequestSite.SiteId);
            loginResult.IsLoginSuccess = true;
            loginResult.CompanyId = loginParams.CompanyId;

            //await WriteLogForLogin(user.UserId, currentRequestSite.SiteId, loginParams);

            return Ok(loginResult);
        }

        private async Task WriteLogForLogin(int userId, int siteId, LoginParam loginModel)
        {
            var command = new InsertUserLoginLogCommand
            {
                UserId = userId,
                SiteId = siteId,
                IPAddress = loginModel.Ip,
                Browser = loginModel.BrowserName,
                Domain = loginModel.Domain,
                Platform = loginModel.Platform
            };

            await userService.UserLoginWriteLogAsync(command);
        }

        private SiteModel GetUserSiteFromRequestHeader()
        {
            try
            {
                string jwtToken = httpContextAccessor.HttpContext.Request.Headers["SiteIdentity"].ToString();
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken.Replace("Bearer ", string.Empty));
                var siteJson = token.Payload.SerializeToJson();
                var site = JsonConvert.DeserializeObject<SiteModel>(siteJson);

                return site;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}