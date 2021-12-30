namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Enums;
    using Core.Models;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.EmployeeWorkFromHome;

    public class EmployeeWorkFromHomeController : BaseController
    {
        private readonly IEmployeeWorkFromHomeService employeeWorkFromHomeService;
        private readonly IBaseService baseService;

        public EmployeeWorkFromHomeController(
            ISessionService sessionService,
            IBaseService baseService,
            IEmployeeWorkFromHomeService employeeWorkFromHomeService) : base(sessionService)
        {
            this.baseService = baseService;
            this.employeeWorkFromHomeService = employeeWorkFromHomeService;
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Index()
        {
            var model = new EmployeeWorkFromHomeFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
            };

            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult GetEmployeeWorkFromHomeGrid(GetEmployeeWorkFromHomeCriteria criteria)
        {
            var employees = baseService.GetData<EmployeeWorkFromHome>(criteria);
            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return PartialView("_EmployeeWorkFromHomeGrid", employees);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            var model = new EmployeeWorkFromHomeViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(EmployeeWorkFromHomeViewModel model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertEmployeeWorkFromHomeCriteria
                {
                    EmpId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = sessionService.Employee.Id,
                    Remark = model.Remark
                };

                result = baseService.ExecuteNonQuery(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            var employeeWorkFromHome = employeeWorkFromHomeService.GetById(id);
            var model = new EmployeeWorkFromHomeViewModel
            {
                EmployeeWorkFromHomeId = employeeWorkFromHome.EmployeeWorkFromHomeId,
                EmployeeId = employeeWorkFromHome.EmpId,
                Employee = string.Format("{0} ({1})", employeeWorkFromHome.Nickname, employeeWorkFromHome.FullName),
                FromDate = employeeWorkFromHome.FromDate,
                ToDate = employeeWorkFromHome.ToDate,
                Remark = employeeWorkFromHome.Remark
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(EmployeeWorkFromHomeViewModel model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateEmployeeWorkFromHomeCriteria
                {
                    EmployeeWorkFromHomeId = model.EmployeeWorkFromHomeId,
                    EmpId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = sessionService.Employee.Id,
                    Remark = model.Remark
                };

                result = baseService.ExecuteNonQuery(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(int id)
        {
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };
            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}