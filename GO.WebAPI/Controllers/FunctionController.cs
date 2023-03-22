namespace GO.WebAPI.Controllers
{
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Models;
    using GO.WebAPI.Criteria.Function;
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
        public async Task<IActionResult> GetBySiteId(GetFunctionBySiteIdCriteria criteria)
        {
            var functions = await baseService.GetDataAsync<Function>(criteria);
            return Json(functions);
        }

        [HttpGet]
        [Route("api/function/getPermissionBySiteAndRoleId")]
        public async Task<IActionResult> GetBySiteAndRoleId(GetFunctionByRoleIdCriteria criteria)
        {
            var permissions = await baseService.GetDataAsync<FunctionPermission>(criteria);
            return Json(permissions);
        }
    }
}