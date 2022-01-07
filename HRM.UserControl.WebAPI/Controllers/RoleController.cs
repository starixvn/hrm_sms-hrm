namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models.Role;
    using HRM.UserControl.WebAPI.Criteria.Role;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class RoleController : BaseController
    {
        private readonly IBaseService baseService;

        public RoleController(
            IHttpContextAccessor httpContextAccessor,
            IBaseService baseService) : base(httpContextAccessor)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        [Route("api/role/getBySiteId")]
        public async Task<IActionResult> GetBySiteId(GetRoleBySiteIdCriteria criteria)
        {
            var roles = await baseService.GetDataAsync<Role>(criteria);
            return Ok(roles);
        }

        [HttpGet]
        [Route("api/role/getByUserId")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var criteria = new GetRoleByUserIdCriteria
            {
                SiteId = SiteId,
                UserId = userId
            };
            var roles = await baseService.GetDataAsync<Role>(criteria);
            return Ok(roles);
        }

        [HttpPost]
        [Route("api/role/assignPermission")]
        public async Task<IActionResult> AssignPermission([FromBody] AssignFunctionToRoleCommand command)
        {
            command.ModifiedBy = UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Ok(commandResult);
        }
    }
}