// --------------------------------------------------------------------------
// <copyright file="DepartmentController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.22">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Department;

    [Authorize]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController()
            : this(null)
        {
        }

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? new DepartmentService();
        }

        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Index(bool? isActive)
        {
            var model = new DepartmentViewModel()
            {
                IsActive = isActive
            };

            return Index(model);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Index(DepartmentViewModel model)
        {
            model.Departments = GetAllDepartment(model.IsActive);

            return View(model);
        }

        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Create()
        {
            var model = new AddDepartmentViewModel();

            model.ParentDepartments = GetAllDepartment(isActive: true);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Create(AddDepartmentViewModel model)
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
                    CreatedBy = this.CurrentUser.Id
                };
                CommandResult result = _departmentService.Add(criteria);

                if (result.Code == 0)
                {
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Edit(int departmentId)
        {
            Department department = GetDepartmentById(departmentId);

            var model = new EditDepartmentViewModel()
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
        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Edit(EditDepartmentViewModel model)
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
                    ModifiedBy = this.CurrentUser.Id,
                };

                CommandResult result = _departmentService.Edit(criteria);

                ViewBag.Updated = result.Code == 0;
                if (result.Code == 0)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public string Delete(int departmentId)
        {
            var criteria = new DeleteDepartmentCriteria
            {
                DepartmentId = departmentId,
                ModifiedBy = this.CurrentUser.Id,
            };
            CommandResult commandResult = _departmentService.Delete(criteria);

            if (commandResult.Code != 0)
            {
                return commandResult.Message;
            }

            return string.Empty;
        }

        private IList<Department> GetAllDepartment(bool? isActive)
        {
            var criteria = new GetDepartmentByParentIdCriteria()
            {
                IsActive = isActive
            };
            IList<Department> departments = _departmentService.GetDepartment(criteria);

            return departments;
        }

        private Department GetDepartmentById(int departmentId)
        {
            var criteria = new GetDepartmentByIdCriteria
            {
                DepartmentId = departmentId
            };

            Department department = _departmentService.GetById(criteria);

            return department;
        }

        private IList<Department> GetParentDepartment(int departmentId)
        {
            var criteria = new GetParentByDepartmentCriteria()
            {
                DepartmentId = departmentId
            };

            IList<Department> departments = _departmentService.GetParentDepartment(criteria);

            return departments;
        }
    }
}