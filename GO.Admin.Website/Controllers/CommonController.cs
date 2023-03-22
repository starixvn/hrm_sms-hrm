namespace GO.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using GO.Admin.Website.Criteria.Site;
    using GO.Admin.Website.Models.Site;
    using GO.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class CommonController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IRoleService roleService;

        public CommonController(
            IBaseService baseService,
            IRoleService roleService)
        {
            this.baseService = baseService;
            this.roleService = roleService;
        }

        public async Task<IActionResult> GetAllSite(GetAllSiteCriteria criteria)
        {
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        public async Task<IActionResult> GetRoleBySiteId(short siteId)
        {
            var roles = await roleService.GetBySiteIdAsync(siteId);
            return Json(roles);
        }
    }
}