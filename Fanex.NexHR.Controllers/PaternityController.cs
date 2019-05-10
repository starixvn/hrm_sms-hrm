namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Enums;
    using Core.Models;
    using Criteria.Paternity;
    using Fanex.NexHR.Services.Interfaces;
    using ViewModels.Paternity;
    using Web.Providers;

    public class PaternityController : BaseController
    {
        private readonly IPaternityService paternityService;

        public PaternityController(IPaternityService paternityService)
        {
            this.paternityService = paternityService;
        }

        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Index()
        {
            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Host);
            return View();
        }

        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult ListPaternity(int year)
        {
            var paternities = paternityService.GetAll(year);
            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Host);
            return PartialView("_ListPaternity", paternities);
        }

        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            var model = new PaternityViewModel
            {
                ChildBornDate = DateTime.Now,
                LeaveDay = 5
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(PaternityViewModel model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertPaternityCriteria
                {
                    EmpId = model.EmployeeId,
                    ChildBornDate = model.ChildBornDate,
                    LeaveDay = model.LeaveDay,
                    CreatedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                result = paternityService.Insert(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            var paternity = paternityService.GetById(id);

            var model = new PaternityViewModel
            {
                EmployeeId = paternity.EmpId,
                Employee = string.Format("{0} ({1})", paternity.Nickname, paternity.Fullname),
                ChildBornDate = paternity.ChildBornDate,
                LeaveDay = paternity.LeaveDay,
                Remark = paternity.Remark
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Update(int id, PaternityViewModel model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdatePaternityCriteria
                {
                    PaternityId = id,
                    EmpId = model.EmployeeId,
                    ChildBornDate = model.ChildBornDate,
                    LeaveDay = model.LeaveDay,
                    ModifiedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                result = paternityService.Update(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(int id)
        {
            var result = paternityService.Delete(id);

            return Json(result);
        }
    }
}