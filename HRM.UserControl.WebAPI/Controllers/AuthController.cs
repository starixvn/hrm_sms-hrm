namespace HRM.UserControl.WebAPI.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria.User;
    using HRM.UserControl.API.Core.Services;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models.User;
    using HRM.UserControl.Models.Auth;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using HRM.UserControl.Models;

    public class AuthController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtManagerService jwtManagerService;
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly ICryptoService cryptoService;

        public AuthController(
            IHttpContextAccessor httpContextAccessor,
            IJwtManagerService jwtManagerService,
            IUserService userService,
            ISiteService siteService,
            ICryptoService cryptoService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.jwtManagerService = jwtManagerService;
            this.userService = userService;
            this.siteService = siteService;
            this.cryptoService = cryptoService;
        }

        [Route("api/auth/login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginParam loginParams)
        {
            string[] keyiv = loginParams.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            loginParams.Username = cryptoService.Decrypt(loginParams.Username, rijndaelKey, rijndaelIV);
            loginParams.Password = cryptoService.Decrypt(loginParams.Password, rijndaelKey, rijndaelIV);

            var loginResult = new LoginResult();
            User user = await userService.GetByUsernameAsync(loginParams.Username);

            if (user is null || !IsValidPassword(loginParams.Password, user))
            {
                loginResult.ErrorMessage = "Invalid username or password.";
                return Ok(loginResult);
            }

            if (!user.IsActive)
            {
                loginResult.ErrorMessage = "User was disabled.";
                return Ok(loginResult);
            }

            if (user.IsDeleted)
            {
                loginResult.ErrorMessage = "User was deleted.";
                return Ok(loginResult);
            }

            var requestSite = GetSiteFromRequestHeader();
            var sites = await siteService.GetByUserIdAsync(user.UserId);
            if (!sites.Any(site => site.SiteId == requestSite.SiteId && site.IsActive))
            {
                loginResult.ErrorMessage = "User do not have permission on this site.";
                return Ok(loginResult);
            }

            loginResult.Token = await jwtManagerService.GenerateTokenAsync(user.UserId, requestSite.SiteId);
            loginResult.IsSuccessfull = true;
            loginResult.UserId = user.UserId;

            await LogUserLoginAsync(user.UserId, requestSite.SiteId, loginParams);

            return Ok(loginResult);
        }

        private async Task<CommandResult> LogUserLoginAsync(int userId, short siteId, LoginParam loginModel)
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

            return await userService.LogUserLoginAsync(command);
        }

        private SiteModel GetSiteFromRequestHeader()
        {
            string jwtToken = httpContextAccessor.HttpContext.Request.Headers["SiteIdentity"].ToString();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken.Replace("Bearer ", string.Empty));
            var siteJson = token.Payload.SerializeToJson();
            return JsonConvert.DeserializeObject<SiteModel>(siteJson);
        }

        private bool IsValidPassword(string plainPwd, User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                return BCrypt.Net.BCrypt.Verify(plainPwd, user.Password);
            }

            return cryptoService.CompareStringToHash(plainPwd, user.OldPassword);
        }
    }
}