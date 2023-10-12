namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria;
    using GO.Website.Criteria.Manpower.Period;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class CommonController : BaseController
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IBaseService baseService;
        private readonly IRoleService roleService;

        public CommonController(
            IUserControlSettings userControlSettings,
            IBaseService baseService,
            IRoleService roleService)
        {
            this.userControlSettings = userControlSettings;
            this.baseService = baseService;
            this.roleService = roleService;
        }

        public async Task<IActionResult> SearchEmployees(SearchEmployeeCriteria criteria)
        {
            var employees = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            return Json(employees);
        }

        [HttpPost]
        public async Task<IActionResult> GetManpowerPeriods()
        {
            var criteria = new GetPeriodCriteria { IsActive = 1 };
            var periods = await baseService.GetDataAsync<ManpowerPeriod>(criteria);

            return Json(periods);
        }

        public async Task<IActionResult> GetRoles()
        {
            IList<Role> roles = await roleService.GetBySiteIdAsync(userControlSettings.SiteId);
            return Json(roles);
        }
    }
}