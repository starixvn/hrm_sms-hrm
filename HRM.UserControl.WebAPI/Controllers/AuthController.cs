namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models;
    using HRM.UserControl.Models.User;
    using HRM.UserControl.Models.Auth;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using HRM.UserControl.WebAPI.Services;
    using HRM.UserControl.WebAPI.Criteria.User;
    using System;
    using Microsoft.AspNetCore.Authorization;

    public class AuthController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtManagerService jwtManagerService;
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly ICryptoService cryptoService;
        private readonly IBaseService baseService;

        public AuthController(
            IHttpContextAccessor httpContextAccessor,
            IJwtManagerService jwtManagerService,
            IUserService userService,
            ISiteService siteService,
            ICryptoService cryptoService,
            IBaseService baseService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.jwtManagerService = jwtManagerService;
            this.userService = userService;
            this.siteService = siteService;
            this.cryptoService = cryptoService;
            this.baseService = baseService;
        }

        [HttpPost]
        [Route("api/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginParam loginParam)
        {
            string[] keyiv = loginParam.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            loginParam.Username = cryptoService.Decrypt(loginParam.Username, rijndaelKey, rijndaelIV);
            loginParam.Password = cryptoService.Decrypt(loginParam.Password, rijndaelKey, rijndaelIV);

            var loginResult = new LoginResult();
            var criteria = new GetUserByUsernameCriteria
            {
                Username = loginParam.Username
            };
            var users = await baseService.GetDataAsync<User>(criteria);
            var validUsers = GetValidUsers(users, loginParam.Password);
            if (validUsers.Count == 0)
            {
                loginResult.ErrorMessage = "Invalid username or password.";
                return Ok(loginResult);
            }

            validUsers = validUsers.Where(x => x.IsActive && !x.IsDeleted).ToList();
            if(validUsers.Count == 0)
            {
                loginResult.ErrorMessage = "User was disabled or deleted.";
                return Ok(loginResult);
            }

            SiteModel requestSite = GetSiteFromRequestHeader();
            var userSites = await GetValidUsersOnSiteAsync(validUsers, requestSite.SiteId);
            if (userSites.Count == 0)
            {
                loginResult.ErrorMessage = "User do not have permission on this site.";
                return Ok(loginResult);
            }

            var user = userSites.First();
            loginResult.JwtToken = await jwtManagerService.GenerateTokenAsync(user.UserId, requestSite.SiteId);
            loginResult.IsSuccessfull = true;
            loginResult.UserId = user.UserId;

            if (loginResult.IsSuccessfull && string.IsNullOrWhiteSpace(user.Password))
            {
                var command = new UpdateUserPasswordCommand
                {
                    UserId = user.UserId,
                    SiteId = requestSite.SiteId,
                    Password = BCrypt.Net.BCrypt.HashPassword(loginParam.Password),
                    IsResetPassword = false,
                    ModifiedBy = user.UserId
                };

                await userService.UpdatePasswordAsync(command);
            }

            loginResult.OnUserId = await UserCheckInAsync(user.UserId, requestSite.SiteId, loginParam);

            await LogUserLoginAsync(user.UserId, requestSite.SiteId, loginParam);

            return Ok(loginResult);
        }

        [HttpGet]
        [Route("api/sso/gettime")]
        public IActionResult GetTime()
        {
            var time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fff");
            return Ok(time);
        }

        [HttpPost]
        [Route("api/sso/login")]
        public async Task<IActionResult> SingleSignOn([FromBody] SSOLoginParam ssoParam)
        {
            var loginResult = new LoginResult();
            SiteModel requestSite = GetSiteFromRequestHeader();
            var userSites = await GetValidUsersOnSiteAsync(new List<User> { new User { UserId = ssoParam.UserId } }, requestSite.SiteId);
            if (userSites.Count == 0)
            {
                loginResult.ErrorMessage = "User do not have permission on this site.";
                return Ok(loginResult);
            }

            var command = new SSOCheckInCommand
            {
                UserId = ssoParam.UserId,
                SiteId = requestSite.SiteId,
                CheckOnUserId = ssoParam.CheckOnUserId,
                IP = ssoParam.IP,
                NetSessionId = ssoParam.NetSessionId
            };
            await userService.SSOCheckInAsync(command);
            await LogUserLoginAsync(ssoParam, requestSite.SiteId);

            loginResult.OnUserId = command.OnUserId;
            loginResult.JwtToken = await jwtManagerService.GenerateTokenAsync(ssoParam.UserId, requestSite.SiteId);
            loginResult.IsSuccessfull = true;

            return Ok(loginResult);
        }

        [HttpPost]
        [Route("api/auth/logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] UserLogoutCommand command)
        {
            await userService.UserLogoutAsync(command);
            return Ok(true);
        }

        [HttpPost]
        [Route("api/auth/recheckin")]
        [Authorize]
        public async Task<IActionResult> ReCheckIn([FromBody] UserReCheckInCommand command)
        {
            await userService.UserReCheckInAsync(command);
            return Ok(command.IsKickedOut ? 1 : 0);
        }

        private async Task<long> UserCheckInAsync(int userId, short siteId, LoginParam loginParam)
        {
            var command = new UserCheckInCommand
            {
                UserId = userId,
                SiteId = siteId,
                IP = loginParam.IP,
                NetSessionId = loginParam.NetSessionId
            };

            await userService.UserCheckInAsync(command);
            return command.OnUserId;
        }

        private async Task<CommandResult> LogUserLoginAsync(int userId, short siteId, LoginParam loginParam)
        {
            var command = new InsertLogUserLoginCommand
            {
                UserId = userId,
                SiteId = siteId,
                IP = loginParam.IP,
                Browser = loginParam.BrowserName,
                Domain = loginParam.Domain,
                Platform = loginParam.Platform
            };

            return await userService.LogUserLoginAsync(command);
        }

        private async Task<CommandResult> LogUserLoginAsync(SSOLoginParam ssoParam, short siteId)
        {
            var command = new InsertLogUserLoginCommand
            {
                UserId = ssoParam.UserId,
                SiteId = siteId,
                IP = ssoParam.IP,
                Browser = ssoParam.BrowserName,
                Domain = ssoParam.Domain,
                Platform = ssoParam.Platform
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

        private IList<User> GetValidUsers(IEnumerable<User> users, string password)
        {
            var validUsers = new List<User>();
            foreach (var user in users)
            {
                if (IsValidPassword(password, user))
                {
                    validUsers.Add(user);
                }
            }

            return validUsers;
        }

        private async Task<IList<User>> GetValidUsersOnSiteAsync(IEnumerable<User> users, short siteId)
        {
            var validUsers = new List<User>();
            foreach (var user in users)
            {
                var sites = await siteService.GetByUserIdAsync(user.UserId);
                if (sites.Any(site => site.SiteId == siteId && site.IsActive))
                {
                    validUsers.Add(user);
                }
            }

            return validUsers;
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