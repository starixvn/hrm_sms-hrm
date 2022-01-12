namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Department;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    public class DepartmentController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;
        private readonly IUnitService unitService;

        public DepartmentController(
            IUserControlSession userControlSession,
            IDepartmentService departmentService,
            IUnitService unitService,
            IBaseService baseService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.departmentService = departmentService;
            this.baseService = baseService;
            this.unitService = unitService;
        }

        [HRMPermission(Policy.Read, FunctionKey.DepartmentManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.DepartmentManagement, Policy.Host);
            IList<Department> departments = departmentService.GetDepartment(isActive);
            return View(departments);
        }

        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public ActionResult Create()
        {
            var model = new CreateDepartmentViewModel
            {
                Units = unitService.GetUnit()
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
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public ActionResult Edit(int Id)
        {
            Department department = departmentService.GetById(Id);
            if (department == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            IList<Unit> units = unitService.GetUnit();
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Id,
                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                UnitId = department.Unit != null ? department.Unit.Id : 0,
                Units = units
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public JsonResult Edit(EditDepartmentViewModel model)
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
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = departmentService.Delete(EmployeeSession.EmpId, id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}