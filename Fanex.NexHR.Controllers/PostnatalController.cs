namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Enums;
    using Core.Models;
    using Criteria.Postnatal;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using ViewModels.Postnatal;

    public class PostnatalController : BaseController
    {
        private readonly IPostnatalService postnatalService;

        public PostnatalController(
            ISessionService sessionService,
            IPostnatalService postnatalService) : base(sessionService)
        {
            this.postnatalService = postnatalService;
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Index()
        {
            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return View();
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult GetPostnatalGrid(int year)
        {
            IList<EmployeePostnatal> postnatals = postnatalService.GetAll(year);
            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return PartialView("_PostnatalGrid", postnatals);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            var model = new PostnatalViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(PostnatalViewModel model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertPostnatalCriteria
                {
                    EmpId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = sessionService.Employee.Id,
                    Remark = model.Remark,
                    WorkingHours = model.WorkingHours ?? 0
                };

                result = postnatalService.Insert(criteria);
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
            EmployeePostnatal postnatal = postnatalService.GetById(id);
            var model = new PostnatalViewModel
            {
                EmployeeId = postnatal.EmpId,
                Employee = string.Format("{0} ({1})", postnatal.Nickname, postnatal.Fullname),
                FromDate = postnatal.FromDate,
                ToDate = postnatal.ToDate,
                Remark = postnatal.Remark,
                WorkingHours = postnatal.WorkingHours
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Update(int id, PostnatalViewModel model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdatePostnatalCriteria
                {
                    PostnatalId = id,
                    EmpId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = sessionService.Employee.Id,
                    Remark = model.Remark,
                    WorkingHours = model.WorkingHours ?? 0
                };

                result = postnatalService.Update(criteria);
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
            var result = postnatalService.Delete(id);

            return Json(result);
        }
    }
}