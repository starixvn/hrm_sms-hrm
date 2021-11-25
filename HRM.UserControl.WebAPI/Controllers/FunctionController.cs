namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria.Function;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class FunctionController : BaseController
    {
        private readonly IBaseService baseService;

        public FunctionController(
            IHttpContextAccessor httpContextAccessor,
            IBaseService baseService) : base(httpContextAccessor)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        [Route("api/function/getBySiteId")]
        public async Task<IActionResult> GetBySiteId([FromBody] GetFunctionBySiteIdCriteria criteria)
        {
            var functions = await baseService.GetDataAsync<Function>(criteria);
            return Ok(functions);
        }

        [HttpGet]
        [Route("api/function/getPermissionByRoleId")]
        public async Task<IActionResult> GetByRoleId(short roleId)
        {
            var criteria = new GetFunctionByRoleIdCriteria
            {
                SiteId = SiteId,
                RoleId = roleId
            };
            var functionPermissions = await baseService.GetDataAsync<FunctionPermission>(criteria);
            return Ok(functionPermissions);
        }
    }
}