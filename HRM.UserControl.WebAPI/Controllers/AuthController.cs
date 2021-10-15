namespace HRM.UserControl.WebAPI.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria.User;
    using HRM.UserControl.API.Core.Services;
    using HRM.UserControl.Models;
    using HRM.UserControl.Models.Auth;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class AuthController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly IJwtManagerService jwtManagerService;

        public AuthController(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            ISiteService siteService,
            IJwtManagerService jwtManagerService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
            this.siteService = siteService;
            this.jwtManagerService = jwtManagerService;
        }

        [Route("api/user/getByUsername")]
        [HttpGet]
        public async Task<IActionResult> GetByUsername(string username)
        {
            User user = await userService.GetByUsernameAsync(username);
            return Ok(user);
        }

        [Route("api/auth/signin")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginParam loginParams)
        {
            LoginResult loginResult = new();
            User user = await userService.GetByIdAsync(loginParams.UserId);

            if (user is null)
            {
                loginResult.ErrorMessage = "User does not exists";
                return Ok(loginResult);
            }

            var requestSite = GetSiteFromRequestHeader();
            var sites = await siteService.GetByUserIdAsync(loginParams.UserId);
            if (!sites.Any(site => site.SiteId == requestSite.SiteId && site.IsActive))
            {
                loginResult.ErrorMessage = "User have not permission on this site";
                return Ok(loginResult);
            }

            loginResult.Token = await jwtManagerService.GenerateTokenAsync(user.UserId, requestSite.SiteId);
            loginResult.IsSuccessfull = true;
            loginResult.UserId = user.UserId;

            //await WriteLogForLogin(user.UserId, currentRequestSite.SiteId, loginParams);

            return Ok(loginResult);
        }

        private async Task WriteLogForLogin(int userId, short siteId, LoginParam loginModel)
        {
            var command = new InsertLogUserLoginCommand
            {
                UserId = userId,
                SiteId = siteId,
                IP = loginModel.IP,
                Browser = loginModel.BrowserName,
                Domain = loginModel.Domain,
                Platform = loginModel.Platform
            };

            await userService.UserLoginWriteLogAsync(command);
        }

        private SiteModel GetSiteFromRequestHeader()
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