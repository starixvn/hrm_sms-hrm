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
                model.Departments = GetAllDepartment(model.IsActive);
            }

            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.DivisionManagement, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Create()
        {
            var model = new AddDepartmentViewModel
            {
                ParentDepartments = GetAllDepartment(isActive: true)
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Create(AddDepartmentViewModel model)
        {
            if (model != null)
            {
                model.ParentDepartments = GetAllDepartment(isActive: true);

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
            Department department = GetDepartmentById(departmentId);

            var model = new EditDepartmentViewModel
            {
                DeprtamentId = departmentId,
                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                ManagerId = department.Manager.Id,
                Manager = department.Manager.DisplayName,
                ParentId = department.Parent == null ? 0 : department.Parent.Id,
                ParentDepartments = GetParentDepartment(departmentId)
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Edit(EditDepartmentViewModel model)
        {
            if (model != null)
            {
                model.ParentDepartments = GetParentDepartment(model.DeprtamentId);
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
            var criteria = new DeleteDepartmentCriteria
            {
                DepartmentId = departmentId,
                ModifiedBy = sessionService.Employee.Id,
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        private IList<Department> GetAllDepartment(bool? isActive)
        {
            var criteria = new GetDepartmentByParentIdCriteria
            {
                IsActive = isActive
            };
            IList<Department> departments = departmentService.GetDepartment(criteria);

            return departments;
        }

        private Department GetDepartmentById(int departmentId)
        {
            var criteria = new GetDepartmentByIdCriteria
            {
                DepartmentId = departmentId
            };

            Department department = departmentService.GetById(criteria);

            return department;
        }

        private IList<Department> GetParentDepartment(int departmentId)
        {
            var criteria = new GetParentByDepartmentCriteria
            {
                DepartmentId = departmentId
            };

            IList<Department> departments = departmentService.GetParentDepartment(criteria);

            return departments;
        }
    }
}