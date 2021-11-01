namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria.Function;
    using HRM.UserControl.API.Core.Criteria.MenuItem;
    using HRM.UserControl.API.Core.Criteria.User;
    using HRM.UserControl.API.Core.Services;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
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
        private readonly IBaseService baseService;

        public UserController(
            IHttpContextAccessor httpContextAccessor,
            IJwtManagerService jwtManagerService,
            IUserService userService,
            IBaseService baseService) : base(httpContextAccessor)
        {
            this.jwtManagerService = jwtManagerService;
            this.userService = userService;
            this.baseService = baseService;
        }

        [HttpPost]
        [Route("api/user/updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdateUserPasswordCommand command)
        {
            command.ModifiedBy = UserId;
            CommandResult commandResult = await userService.UpdatePasswordAsync(command);
            return Ok(commandResult);
        }

        [HttpPost]
        [Route("api/refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            string token = await jwtManagerService.GenerateTokenAsync(UserId, SiteId);
            return Ok(JsonConvert.SerializeObject(token));
        }

        [HttpGet]
        [Route("api/user/getMenuItem")]
        public async Task<IActionResult> GetMenuItem()
        {
            var criteria = new GetMenuItemByUserIdCriteria
            {
                UserId = UserId,
                SiteId = SiteId
            };
            var menuItems = await baseService.GetDataAsync<MenuItem>(criteria);
            return Ok(menuItems);
        }

        [HttpGet]
        [Route("api/user/getFunctionPermission")]
        public async Task<IActionResult> GetFunctionPermission()
        {
            var criteria = new GetFunctionByUserIdCriteria
            {
                UserId = UserId,
                SiteId = SiteId
            };
            var functions = await baseService.GetDataAsync<FunctionPermission>(criteria);
            return Ok(functions);
        }
    }
}