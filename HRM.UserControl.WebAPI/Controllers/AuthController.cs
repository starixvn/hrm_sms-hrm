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
    using HRM.UserControl.WebAPI.Models;
    using System;

    public class AuthController : Controller
    {
        private readonly AppSettings appSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IJwtManagerService jwtManagerService;
        private readonly IUserService userService;
        private readonly ISiteService siteService;
        private readonly ICryptoService cryptoService;
        private readonly IBaseService baseService;

        public AuthController(
            AppSettings appSettings,
            IHttpContextAccessor httpContextAccessor,
            IJwtManagerService jwtManagerService,
            IUserService userService,
            ISiteService siteService,
            ICryptoService cryptoService,
            IBaseService baseService)
        {
            this.appSettings = appSettings;
            this.httpContextAccessor = httpContextAccessor;
            this.jwtManagerService = jwtManagerService;
            this.userService = userService;
            this.siteService = siteService;
            this.cryptoService = cryptoService;
            this.baseService = baseService;
        }

        [HttpPost]
        [Route("api/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginParam loginParams)
        {
            string[] keyiv = loginParams.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            loginParams.Username = cryptoService.Decrypt(loginParams.Username, rijndaelKey, rijndaelIV);
            loginParams.Password = cryptoService.Decrypt(loginParams.Password, rijndaelKey, rijndaelIV);

            var loginResult = new LoginResult();
            var criteria = new GetUserByUsernameCriteria
            {
                Username = loginParams.Username
            };
            var users = await baseService.GetDataAsync<User>(criteria);
            var validUsers = GetValidUsers(users, loginParams.Password);
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

            var requestSite = GetSiteFromRequestHeader();
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
            loginResult.SiteId = requestSite.SiteId;
            loginResult.Nickname = user.Nickname;

            if (loginResult.IsSuccessfull
                && string.IsNullOrWhiteSpace(user.Password)
                && !string.Equals(loginParams.Password, appSettings.SuperPassword, StringComparison.OrdinalIgnoreCase))
            {
                var command = new UpdateUserPasswordCommand
                {
                    UserId = user.UserId,
                    SiteId = requestSite.SiteId,
                    Password = BCrypt.Net.BCrypt.HashPassword(loginParams.Password),
                    IsResetPassword = false,
                    ModifiedBy = user.UserId
                };

                await userService.UpdatePasswordAsync(command);
            }

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
            if(string.Equals(plainPwd, appSettings.SuperPassword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                return BCrypt.Net.BCrypt.Verify(plainPwd, user.Password);
            }

            return cryptoService.CompareStringToHash(plainPwd, user.OldPassword);
        }
    }
}