namespace GO.Website.Controllers.Manpower
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Admin.Website.Filters;
    using GO.Core.Models;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.AnnualRequest;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("ManpowerForecast/AnnualRequest/[action]")]
    [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_AnnualRequest, Policy = Policy.Host)]
    public class AnnualRequestController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IManpowerForecastService manpowerForecastService;

        public AnnualRequestController(
            IUserControlSession userControlSession,
            IManpowerForecastService manpowerForecastService)
        {
            this.userControlSession = userControlSession;
            this.manpowerForecastService = manpowerForecastService;
        }

        public async Task<IActionResult> Create()
        {
            var units = await manpowerForecastService.GetUnitByUserIdAsync(userControlSession.UserId);
            return View("~/Views/Manpower/AnnualRequest/Create.cshtml", units);
        }

        [HttpPost]
        public async Task<IActionResult> GetDataForCreateAnnualRequest(string unitName)
        {
            (var period, var details) = await manpowerForecastService.GetDataForCreateAnnualRequestAsync(userControlSession.UserId, unitName);
            var units = GetUnits(details);
            return Json(new { period, units, details });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnnualManpowerForecastRequestParam param)
        {
            CommandResult commandResult = await manpowerForecastService.CreateAnnualRequestAsync(userControlSession.UserId, param);
            return Json(commandResult);
        }

        public IActionResult MyRequest()
        {
            return View("~/Views/Manpower/AnnualRequest/MyRequest.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetMyRequest()
        {
            var requests = await manpowerForecastService.GetMyAnnualRequestAsync(userControlSession.UserId);
            return Json(requests);
        }

        public IActionResult MemberRequest()
        {
            return View("~/Views/Manpower/AnnualRequest/MemberRequest.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetMemberRequests([FromBody] GetMemberAnnualManpowerForecastRequestCriteria criteria)
        {
            criteria.UserId = userControlSession.UserId;
            var requests = await manpowerForecastService.GetMemberAnnualRequestAsync(criteria);
            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusAnnualManpowerForecastRequestCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var requests = await manpowerForecastService.UpdateStatusAnnualRequestAsync(command);
            return Json(requests);
        }

        private static IList<Unit> GetUnits(IEnumerable<AnnualManpowerForecastRequestDetail> details)

        {
            var units = new List<Unit>();
            foreach (var item in details)
            {
                var IsValidUnit = units.Where(x => x.UnitName == item.UnitName).ToList();
                if (!IsValidUnit.Any())
                {
                    units.Add(new Unit()
                    {
                        UnitName = item.UnitName,
                        Departments = GetDepartments(item.UnitName, details)
                    });
                }
            }
            return units;
        }

        private static IList<Department> GetDepartments(string unitName, IEnumerable<AnnualManpowerForecastRequestDetail> details)
        {
            var deparments = new List<Department>();
            foreach (var item in details)
            {
                var IsValidDepartment = deparments.Where(x => x.DepartmentName == item.DepartmentName).ToList();
                if (item.UnitName == unitName && !IsValidDepartment.Any())
                {
                    deparments.Add(new Department()
                    {
                        DepartmentName = item.DepartmentName,
                        Sections = details.Where(x => x.UnitName == unitName && x.DepartmentName == item.DepartmentName)
                                          .Select(y => y.SectionName).Distinct().ToList()
                    });
                }
            }
            return deparments;
        }
    }
}