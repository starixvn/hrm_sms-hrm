namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.NewDepartment;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.NewDepartment;

    public class NewDepartmentController : BaseController
    {
        private readonly INewDepartmentService departmentService;
        private readonly IBaseService baseService;

        public NewDepartmentController(
            ISessionService sessionService,
            INewDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.departmentService = departmentService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.DepartmentManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            IList<NewDepartment> departments = departmentService.GetByActive(isActive);
            return View(departments);
        }

        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public ActionResult Create()
        {
            var model = new CreateDepartmentViewModel
            {
                Unit = departmentService.GetUnit()
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public JsonResult Create(CreateDepartmentViewModel model)
        {
            var commandResult = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertDepartmentCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    UnitId = model.UnitId,
                    ModifiedBy = sessionService.Employee.Id
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public ActionResult Edit(int departmentId)
        {
            NewDepartment department = departmentService.GetById(departmentId);
            IList<Unit> unit = departmentService.GetUnit();
            var model = new EditNewDepartmentViewModel
            {
                DepartmentId = departmentId,
                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                UnitId = department.Unit.Id,
                Unit = unit
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public JsonResult Edit(EditNewDepartmentViewModel model)
        {
            var commandResult = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateDepartmentCriteria
                {
                    DepartmentId = model.DepartmentId,
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    UnitId = model.UnitId,
                    ModifiedBy = sessionService.Employee.Id
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = departmentService.Delete(id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}