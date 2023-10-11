namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Section;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    public class SectionController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly ICommonDataService commonDataService;
        private readonly IUnitService unitService;
        private readonly ISectionService sectionService;
        private readonly IBaseService baseService;
        private readonly IDepartmentService departmentService;

        public SectionController(
            IUnitService unitService,
            IUserControlSession userControlSession,
            ICommonDataService commonDataService,
            IDepartmentService departmentService,
            ISectionService sectionService,
            IBaseService baseService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.commonDataService = commonDataService;
            this.sectionService = sectionService;
            this.baseService = baseService;
            this.departmentService = departmentService;
            this.unitService = unitService;
        }

        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index()
        {
            var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);

            return View(hasPermission);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index(SectionViewModel model)
        {
            if (model != null)
            {
                model.Sections = sectionService.GetSection(isActive: model.IsActive);
            }

            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create()
        {
            var model = new AddSectionViewModel
            {
                Departments = departmentService.GetDepartment()
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public JsonResult Create(AddSectionViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertSectionCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    DepartmentId = model.DepartmentId,
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Edit(int Id)
        {
            Section section = sectionService.GetById(Id);
            if (section == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            IList<Department> departments = departmentService.GetDepartment();
            var model = new EditSectionViewModel
            {
                SectionId = Id,
                Code = section.Code,
                Name = section.Name,
                IsActive = section.IsActive,
                DepartmentId = section.Department != null ? section.Department.Id : 0,
                Departments = departments
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public JsonResult Edit(EditSectionViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateSectionCriteria
                {
                    SectionId = model.SectionId,
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    DepartmentId = model.DepartmentId,
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = sectionService.Delete(EmployeeSession.EmpId, id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetOrgChart()
        {
            IList<Unit> units = unitService.GetUnit();
            IList<Department> department = departmentService.GetDepartment();
            IList<Team> teams = commonDataService.GetAllTeam();
            IList<Section> sections = sectionService.GetSection();

            IEnumerable<MultiSelectViewModel> orgchart = GetOrgChart(units, department, teams, sections);

            return new CustomJsonResult { Data = orgchart, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IEnumerable<MultiSelectViewModel> GetOrgChart(
            IList<Unit> units,
            IList<Department> department,
            IList<Team> teams,
            IList<Section> sections)
        {
            var multiSelectViewModel = units.Select(u => new MultiSelectViewModel
            {
                Id = u.Id,
                Text = u.Name,
                Items = department.Where(d => d.Unit != null && d.Unit.Id == u.Id).Select(d => new MultiSelectViewModel
                {
                    Id = d.Id,
                    Text = d.Name,
                    ParentId = u.Id,
                    Items = sections.Where(s => s.Department != null && s.Department.Id == d.Id).Select(s => new MultiSelectViewModel
                    {
                        Id = s.Id,
                        Text = s.Name,
                        ParentId = d.Id,
                        Items = teams.Where(t => t.Section != null && t.Section.Id == s.Id).Select(t => new MultiSelectViewModel
                        {
                            Id = t.Id,
                            Text = t.Name,
                            IsLeaf = true,
                            ParentId = s.Id,
                        }).ToList()
                    }).ToList()
                }).ToList()
            });

            return multiSelectViewModel;
        }
    }
}