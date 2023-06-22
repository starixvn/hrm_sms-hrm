using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.Core.Services;
using GO.Website.Constants;
using GO.Website.Models.Manpower;
using GO.Website.Models.Manpower.AnnualRequest;
using GO.Website.Services;
using HRM.UserControl.Client.Core.Enumerables;
using HRM.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;

namespace GO.Website.Controllers.Manpower
{
    [Route("Manpower/AnnualRequest/[action]")]
    public class AnnualRequestController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;
        private readonly IManpowerForecastService manpowerForecastService;

        public AnnualRequestController(
            IUserControlSession userControlSession,
            IManpowerForecastService manpowerForecastService,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.manpowerForecastService = manpowerForecastService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_PeriodManagement, Policy.Host);
            return View("~/Views/Manpower/AnnualRequest/Index.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetDataForCreateAnnualRequest()
        {
            (var period, var annualRequestFormDetails) = await manpowerForecastService.GetDataForCreateAnnualRequestAsync(userControlSession.UserId);
            var companies = GetCompanies(annualRequestFormDetails);
            var units = GetUnits(annualRequestFormDetails);
            return Json(new { period, units, annualRequestFormDetails, companies });
        }

        private static IList<Company> GetCompanies(IEnumerable<AnnualRequestFormDetail> annualRequestFormDetails)
        {
            var companies = new List<Company>();

            foreach (var item in annualRequestFormDetails)
            {
                var IsValidCompany = companies.Where(x => x.Name == item.CompanyName).ToList();
                if (!IsValidCompany.Any())
                {
                    companies.Add(new Company()
                    {
                        Name = item.CompanyName,
                        Units = annualRequestFormDetails.Where(x => x.CompanyName == item.CompanyName).Select(y => y.UnitName).Distinct().ToList(),
                        Departments = annualRequestFormDetails.Where(x => x.CompanyName == item.CompanyName).Select(y => y.DepartmentName).Distinct().ToList(),
                        Sections = annualRequestFormDetails.Where(x => x.CompanyName == item.CompanyName).Select(y => y.SectionName).Distinct().ToList()
                    });
                }
            }
            return companies;
        }

        private static IList<Unit> GetUnits(IEnumerable<AnnualRequestFormDetail> annualRequestFormDetails)
        {
            var units = new List<Unit>();
            foreach (var item in annualRequestFormDetails)
            {
                var IsValidUnit = units.Where(x => x.UnitName == item.UnitName).ToList();
                if (!IsValidUnit.Any())
                {
                    units.Add(new Unit()
                    {
                        UnitName = item.UnitName,
                        Departments = GetDepartments(item.UnitName, annualRequestFormDetails)
                    });
                }
            }
            return units;
        }

        private static IList<Department> GetDepartments(string unitName, IEnumerable<AnnualRequestFormDetail> annualRequestFormDetails)
        {
            var deparments = new List<Department>();
            foreach (var item in annualRequestFormDetails)
            {
                var IsValidDepartment = deparments.Where(x => x.DepartmentName == item.DepartmentName).ToList();
                if (item.UnitName == unitName && !IsValidDepartment.Any())
                {
                    deparments.Add(new Department()
                    {
                        DepartmentName = item.DepartmentName,
                        Sections = GetSection(unitName, item.DepartmentName, annualRequestFormDetails)
                    });

                }
            }
            return deparments;
        }

        private static IList<Section> GetSection(string unitName, string departmentName, IEnumerable<AnnualRequestFormDetail> annualRequestFormDetails)
        {
            var sections = new List<Section>();
            foreach (var item in annualRequestFormDetails)
            {
                var IsValidDepartment = sections.Where(x => x.SectionName == item.SectionName).ToList();
                if (item.UnitName == unitName && item.DepartmentName == departmentName && !IsValidDepartment.Any())
                {
                    sections.Add(new Section()
                    {
                        SectionName = item.SectionName,
                        JobLevels = annualRequestFormDetails.Where(x => x.UnitName == unitName &&
                                                                x.DepartmentName == departmentName &&
                                                                x.SectionName == item.SectionName).Select(y => y.JobLevel).ToList()
                    });

                }
            }
            return sections;
        }
    }
}
