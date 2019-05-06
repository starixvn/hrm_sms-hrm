namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Criteria.Postnatal;
    using Models;
    using Services.Interfaces;
    using ViewModels.Postnatal;

    public class PostnatalController : BaseController
    {
        private readonly IPostnatalService postnatalService;

        public PostnatalController(IPostnatalService postnatalService)
        {
            this.postnatalService = postnatalService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListPostnatal(int year)
        {
            var postnatals = postnatalService.GetAll(year);
            return PartialView("_ListPostnatal", postnatals);
        }

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
        [ValidateInput(true)]
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
                    CreatedBy = CurrentUser.Id
                };

                result = postnatalService.Insert(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        public ActionResult Edit(int id)
        {
            var postnatal = postnatalService.GetById(id);
            var model = new PostnatalViewModel
            {
                EmployeeId = postnatal.EmpId,
                Employee = string.Format("{0} ({1})", postnatal.Nickname, postnatal.Fullname),
                FromDate = postnatal.FromDate,
                ToDate = postnatal.ToDate
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
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
                    ModifiedBy = CurrentUser.Id
                };

                result = postnatalService.Update(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = postnatalService.Delete(id);
            if (result.IsSuccessful)
            {
                return RedirectToAction("Index");
            }

            return Json(result);
        }

        public JsonResult GetMemberToAddPostnatal(string keyword)
        {
            var criteria = new GetMemberToAddPostnatalCriteria
            {
                SupervisorId = CurrentUser.Id,
                Query = keyword
            };

            IList<Employee> employees = postnatalService.GetEmployeeToAddPostnatal(criteria);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }
    }
}