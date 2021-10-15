namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria.User;
    using HRM.UserControl.API.Core.Services;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Authorize]
    public class UserController : BaseController
    {
        private readonly IJwtManagerService jwtManagerService;
        private readonly IUserService userService;
        private readonly ISiteService siteService;

        public UserController(
            IJwtManagerService jwtManagerService,
            IUserService userService,
            ISiteService siteService,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this.jwtManagerService = jwtManagerService;
            this.userService = userService;
            this.siteService = siteService;
        }

        [HttpGet]
        [Route("api/user/getById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            User user = await userService.GetByIdAsync(userId);
            return Ok(user);
        }

        [HttpPost]
        [Route("api/user/updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdateUserPasswordCommand command)
        {
            CommandResult commandResult = await userService.UpdatePasswordAsync(command);
            return Ok(commandResult);
        }

        [HttpGet]
        [Route("api/user/getSite")]
        public async Task<IActionResult> GetSite()
        {
            IEnumerable<SiteModel> sites = await siteService.GetByUserIdAsync(UserId);
            return Ok(sites);
        }

        [HttpPost]
        [Route("api/refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            string token = await jwtManagerService.GenerateTokenAsync(UserId, SiteId);
            return Ok(JsonConvert.SerializeObject(token));
        }
    }
}