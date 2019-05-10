namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Enums;
    using Core.Models;
    using Criteria.Postnatal;
    using Services.Interfaces;
    using ViewModels.Postnatal;
    using Web.Providers;

    public class PostnatalController : BaseController
    {
        private readonly IPostnatalService postnatalService;

        public PostnatalController(IPostnatalService postnatalService)
        {
            this.postnatalService = postnatalService;
        }

        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Index()
        {
            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Host);
            return View();
        }

        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult ListPostnatal(int year)
        {
            var postnatals = postnatalService.GetAll(year);
            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Host);
            return PartialView("_ListPostnatal", postnatals);
        }

        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            var model = new PostnatalViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddYears(1)
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
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
                    CreatedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                result = postnatalService.Insert(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            var postnatal = postnatalService.GetById(id);
            var model = new PostnatalViewModel
            {
                EmployeeId = postnatal.EmpId,
                Employee = string.Format("{0} ({1})", postnatal.Nickname, postnatal.Fullname),
                FromDate = postnatal.FromDate,
                ToDate = postnatal.ToDate,
                Remark = postnatal.Remark
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
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
                    ModifiedBy = CurrentUser.Id,
                    Remark = model.Remark
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
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(int id)
        {
            var result = postnatalService.Delete(id);

            return Json(result);
        }
    }
}