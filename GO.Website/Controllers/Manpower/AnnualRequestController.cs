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
    [GOPermission(FunctionKey = FunctionKey.Manpower_PeriodManagement, Policy = Policy.Host)]
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

        public IActionResult Create()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_PeriodManagement, Policy.Host);
            return View("~/Views/Manpower/AnnualRequest/Create.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetDataForCreateAnnualRequest()
        {
            (var period, var annualRequestFormDetails) = await manpowerForecastService.GetDataForCreateAnnualRequestAsync(userControlSession.UserId);
            var companies = GetCompanies(annualRequestFormDetails);
            var units = GetUnits(annualRequestFormDetails);
            return Json(new { period, units, annualRequestFormDetails, companies });
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
        public async Task<IActionResult> GetMemberRequests(GetMemberAnnualManpowerForecastRequestCriteria criteria)
        {
            criteria.UserId = userControlSession.UserId;
            var requests = await manpowerForecastService.GetMemberAnnualRequestAsync(criteria);
            return Json(requests);
        }

        private static IList<Company> GetCompanies(IEnumerable<AnnualManpowerForecastRequestDetail> details)
        {
            var companies = new List<Company>();

            foreach (var item in details)
            {
                var IsValidCompany = companies.Where(x => x.Name == item.CompanyName).ToList();
                if (!IsValidCompany.Any())
                {
                    companies.Add(new Company
                    {
                        Name = item.CompanyName,
                        Units = details.Where(x => x.CompanyName == item.CompanyName).Select(y => y.UnitName).Distinct().ToList(),
                        Departments = details.Where(x => x.CompanyName == item.CompanyName).Select(y => y.DepartmentName).Distinct().ToList(),
                        Sections = details.Where(x => x.CompanyName == item.CompanyName).Select(y => y.SectionName).Distinct().ToList()
                    });
                }
            }
            return companies;
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
                        Sections = GetSection(unitName, item.DepartmentName, details)
                    });
                }
            }
            return deparments;
        }

        private static IList<Section> GetSection(string unitName, string departmentName, IEnumerable<AnnualManpowerForecastRequestDetail> details)
        {
            var sections = new List<Section>();
            foreach (var item in details)
            {
                var IsValidDepartment = sections.Where(x => x.SectionName == item.SectionName).ToList();
                if (item.UnitName == unitName && item.DepartmentName == departmentName && !IsValidDepartment.Any())
                {
                    sections.Add(new Section()
                    {
                        SectionName = item.SectionName,
                        JobLevels = details.Where(x => x.UnitName == unitName &&
                                                                x.DepartmentName == departmentName &&
                                                                x.SectionName == item.SectionName).Select(y => y.JobLevel).ToList()
                    });
                }
            }
            return sections;
        }
    }
}