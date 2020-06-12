namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Section;

    public class SectionController : BaseController
    {
        private readonly ICommonDataService commonDataService;
        private readonly IUnitService unitService;
        private readonly ISectionService sectionService;
        private readonly IBaseService baseService;
        private readonly IDepartmentService departmentService;

        public SectionController(
            IUnitService unitService,
            ISessionService sessionService,
            ICommonDataService commonDataService,
            IDepartmentService departmentService,
            ISectionService sectionService,
            IBaseService baseService) : base(sessionService)
        {
            this.commonDataService = commonDataService;
            this.sectionService = sectionService;
            this.baseService = baseService;
            this.departmentService = departmentService;
            this.unitService = unitService;
        }

        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);
            var model = new SectionViewModel
            {
                IsActive = isActive,
                Sections = sectionService.GetSection(isActive: isActive)
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index(SectionViewModel model)
        {
            if (model != null)
            {
                model.Sections = sectionService.GetSection(isActive: model.IsActive);
            }

            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);

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
                    ModifiedBy = sessionService.Employee.Id
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Edit(int Id)
        {
            Section section = sectionService.GetById(Id);
            IList<Department> department = departmentService.GetDepartment();
            var model = new EditSectionViewModel
            {
                SectionId = Id,
                Code = section.Code,
                Name = section.Name,
                IsActive = section.IsActive,
                DepartmentId = section.Department.Id,
                Departments = department
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
                    ModifiedBy = sessionService.Employee.Id
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = sectionService.Delete(id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetHierarchy()
        {
            IList<Unit> units = unitService.GetUnit();
            IList<Department> department = departmentService.GetDepartment();
            IList<Team> teams = commonDataService.GetAllTeam();
            IList<Section> sections = sectionService.GetSection();

            IEnumerable<MultiSelectViewModel> hierarchy = GetHierarchy(units, department, teams, sections);

            return new CustomJsonResult { Data = hierarchy, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IEnumerable<MultiSelectViewModel> GetHierarchy(IList<Unit> units, IList<Department> department, IList<Team> teams, IList<Section> sections)
        {
            IEnumerable<MultiSelectViewModel> multiSelectViewModel = units.Select(u => new MultiSelectViewModel
            {
                Id = u.Id,
                Text = u.Name,
                Type = Labels.Unit,
                Items = department.Where(d => d.Unit.Id == u.Id).Select(d => new MultiSelectViewModel
                {
                    Id = d.Id,
                    Text = d.Name,
                    ParentId = u.Id,
                    Type = Labels.Department,
                    Items = sections.Where(s => s.Department.Id == d.Id).Select(s => new MultiSelectViewModel
                    {
                        Id = s.Id,
                        Text = s.Name,
                        ParentId = d.Id,
                        Type = Labels.Section,
                        Items = teams.Where(t => t.Section.Id == s.Id).Select(t => new MultiSelectViewModel
                        {
                            Id = t.Id,
                            Text = t.Name,
                            IsLeaf = true,
                            ParentId = s.Id,
                            Type = Labels.Team,
                        }).ToList()
                    }).ToList()
                }).ToList()
            });

            return multiSelectViewModel;
        }
    }
}