namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
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
        public async Task<IActionResult> GetManpowerPeriods([FromBody] GetPeriodCriteria criteria)
        {
            var (periods, processSettings) = await baseService.FetchMultipleAsync<ManpowerPeriod, MapnowerPeriodProcessSetting>(criteria);
            foreach (var period in periods)
            {
                period.Settings = processSettings.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return Json(periods);
        }

        public async Task<IActionResult> GetRoles()
        {
            IList<Role> roles = await roleService.GetBySiteIdAsync(userControlSettings.SiteId);
            return Json(roles);
        }
    }
}