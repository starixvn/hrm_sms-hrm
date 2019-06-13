namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Department;

    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;

        public DepartmentController(
            ISessionService sessionService,
            IDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.departmentService = departmentService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.DivisionManagement)]
        public ActionResult Index(bool? isActive)
        {
            var model = new DepartmentViewModel
            {
                IsActive = isActive
            };

            return Index(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.DivisionManagement)]
        public ActionResult Index(DepartmentViewModel model)
        {
            if (model != null)
            {
                model.Departments = departmentService.GetDepartment(isActive: model.IsActive);
            }

            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.DivisionManagement, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Create()
        {
            var model = new AddDepartmentViewModel
            {
                ParentDepartments = departmentService.GetDepartment()
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Create(AddDepartmentViewModel model)
        {
            if (model != null)
            {
                model.ParentDepartments = departmentService.GetDepartment();

                if (ModelState.IsValid)
                {
                    var criteria = new InsertDepartmentCriteria
                    {
                        Code = model.Code,
                        Name = model.Name,
                        IsActive = model.IsActive,
                        ParentId = model.ParentId,
                        ManagerId = model.ManagerId,
                        CreatedBy = sessionService.Employee.Id
                    };
                    CommandResult result = baseService.ExecuteNonQuery(criteria);

                    if (result.IsSuccessful)
                    {
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Edit(int departmentId)
        {
            Department department = departmentService.GetById(departmentId);
            IList<Department> parentDepartments = departmentService.GetParentDepartment(departmentId);
            var model = new EditDepartmentViewModel
            {
                DeprtamentId = departmentId,
                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                ManagerId = department.Manager.Id,
                Manager = department.Manager.DisplayName,
                ParentId = department.Parent == null ? 0 : department.Parent.Id,
                ParentDepartments = parentDepartments
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Edit(EditDepartmentViewModel model)
        {
            if (model != null)
            {
                model.ParentDepartments = departmentService.GetParentDepartment(model.DeprtamentId);
                if (ModelState.IsValid)
                {
                    var criteria = new UpdateDepartmentCriteria
                    {
                        DepartmentId = model.DeprtamentId,
                        Code = model.Code,
                        Name = model.Name,
                        IsActive = model.IsActive,
                        ParentId = model.ParentId ?? 0,
                        ManagerId = model.ManagerId,
                        ModifiedBy = sessionService.Employee.Id,
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria);

                    if (result.IsSuccessful)
                    {
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public JsonResult Delete(int departmentId)
        {
            CommandResult result = departmentService.Delete(sessionService.Employee.Id, departmentId);

            return Json(result);
        }
    }
}