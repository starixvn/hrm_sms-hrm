namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Department;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService departmentService;
        private readonly ICommandService commandService;

        public DepartmentController()
            : this(null, null)
        {
        }

        public DepartmentController(IDepartmentService departmentService, ICommandService commandService)
        {
            this.departmentService = departmentService ?? new DepartmentService();
            this.commandService = commandService ?? new CommandService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.DivisionManagement)]
        public ActionResult Index(bool? isActive)
        {
            var model = new DepartmentViewModel
            {
                IsActive = isActive
            };

            return Index(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Read, FunctionKey.DivisionManagement)]
        public ActionResult Index(DepartmentViewModel model)
        {
            if (model != null)
            {
                model.Departments = GetAllDepartment(model.IsActive);
            }

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.DivisionManagement, Policy.Host);

            return View(model);
        }

        [HRAuthorize(Policy.Host, FunctionKey.DivisionManagement)]
        public ActionResult Create()
        {
            var model = new AddDepartmentViewModel();

            model.ParentDepartments = GetAllDepartment(isActive: true);
            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.DivisionManagement)]
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
                        CreatedBy = CurrentUser.Id
                    };
                    CommandResult result = commandService.Insert(criteria);

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

        [HRAuthorize(Policy.Host, FunctionKey.DivisionManagement)]
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
        [HRAuthorize(Policy.Host, FunctionKey.DivisionManagement)]
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
                        ParentId = model.ParentId.HasValue ? model.ParentId.Value : 0,
                        ManagerId = model.ManagerId,
                        ModifiedBy = CurrentUser.Id,
                    };

                    CommandResult result = commandService.Update(criteria);

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
        [HRAuthorize(Policy.Host, FunctionKey.DivisionManagement)]
        public JsonResult Delete(int departmentId)
        {
            var criteria = new DeleteDepartmentCriteria
            {
                DepartmentId = departmentId,
                ModifiedBy = CurrentUser.Id,
            };
            CommandResult result = commandService.Delete(criteria);
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