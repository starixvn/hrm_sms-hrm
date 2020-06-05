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
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Section;

    public class SectionController : BaseController
    {
        private readonly ICommonDataService commonDataService;
        private readonly ISectionService sectionService;
        private readonly IBaseService baseService;
        private readonly INewDepartmentService departmentService;

        public SectionController(
            ISessionService sessionService,
            ICommonDataService commonDataService,
            INewDepartmentService departmentService,
            ISectionService sectionService,
            IBaseService baseService) : base(sessionService)
        {
            this.commonDataService = commonDataService;
            this.sectionService = sectionService;
            this.baseService = baseService;
            this.departmentService = departmentService;
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
                Departments = departmentService.GetByActive()
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
            IList<NewDepartment> department = departmentService.GetByActive();
            var model = new EditSectionViewModel
            {
                SectionId = Id,
                Code = section.Code,
                Name = section.Name,
                IsActive = section.IsActive,
                DepartmentId= section.Department.Id,
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
    }
}