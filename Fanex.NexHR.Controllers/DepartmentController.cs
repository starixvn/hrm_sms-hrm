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
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Department;

    public class DepartmentController : BaseController
    {
        private readonly ICommonDataService commonDataService;
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;

        public DepartmentController(
            ISessionService sessionService,
            ICommonDataService commonDataService,
            IDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.commonDataService = commonDataService;
            this.departmentService = departmentService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.DivisionManagement)]
        public ActionResult Index(bool? isActive = null)
        {
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.DivisionManagement, Policy.Host);
            var model = new DepartmentViewModel
            {
                IsActive = isActive,
                Departments = departmentService.GetDepartment(isActive: isActive)
            };

            return View(model);
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
            IList<Department> parentDepartments = departmentService.GetDepartment(); 
            parentDepartments = parentDepartments.Where(t => t.Id != department.Id).ToList();

            var model = new EditDepartmentViewModel
            {
                DeprtamentId = departmentId,

                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                ManagerId = department.Manager.EmpId,
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

        public CustomJsonResult GetHierarchy()
        {
            IList<Team> sections = commonDataService.GetAllSection();
            IEnumerable<MultiSelectViewModel> departments = departmentService.GetDepartment().Select(x => new MultiSelectViewModel
            {
                Id = x.Id,
                Text = x.Name,
                ParentId = x.Parent == null ? (int?)null : x.Parent.Id,
                Items = sections.Where(y => y.Department != null && y.Department.Id == x.Id).Select(y => new MultiSelectViewModel
                {
                    Id = y.Id,
                    Text = y.Name,
                    IsLeaf = true
                }).ToList()
            }).ToList();
            IEnumerable<MultiSelectViewModel> hierarchy = GetHierarchy(departments);

            return new CustomJsonResult { Data = hierarchy, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IEnumerable<MultiSelectViewModel> GetHierarchy(IEnumerable<MultiSelectViewModel> departments)
        {
            IEnumerable<MultiSelectViewModel> parents = departments.Where(x => !x.ParentId.HasValue);
            foreach (MultiSelectViewModel parent in parents)
            {
                GetChilds(parent, departments);
            }

            return parents;
        }

        private void GetChilds(MultiSelectViewModel department, IEnumerable<MultiSelectViewModel> departments)
        {
            IEnumerable<MultiSelectViewModel> items = departments.Where(x => x.ParentId.HasValue && x.ParentId.Value == department.Id);

            if (items.Any())
            {
                foreach (MultiSelectViewModel item in items)
                {
                    department.Items.Add(item);
                    GetChilds(item, departments);
                }
            }
        }
    }
}