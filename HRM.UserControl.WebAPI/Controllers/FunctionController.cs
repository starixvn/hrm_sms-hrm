namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria.MenuItem;
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

        [HttpPost]
        [Route("api/menuItem/getByUserId")]
        public async Task<IActionResult> UpdatePassword(int userId)
        {
            var criteria = new GetMenuItemByUserIdCriteria
            {
                UserId = userId,
                SiteId = SiteId
            };
            var menuItems = await baseService.GetDataAsync<MenuItem>(criteria);
            return Ok(menuItems);
        }
    }
}