namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Services;
    using Criteria.Holidays;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Web.Providers;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;
        private readonly IBaseService baseService;

        public HolidayController(
            IHolidayService holidayService,
            IBaseService baseService)
        {
            this.holidayService = holidayService;
            this.baseService = baseService;
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.Holiday)]
        public ActionResult Index(int? year)
        {
            int yearToCheck = year.HasValue ? year.Value : DateTime.Now.Year;

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Host);
            ViewBag.Year = yearToCheck;

            IList<Holiday> holidays = holidayService.GetByYear(yearToCheck);

            return View(holidays);
        }

        [HRAuthorize(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Edit(int id)
        {
            Holiday holiday = holidayService.GetById(id);

            return View(holiday);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Edit(Holiday model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateHolidayCriteria
                {
                    Id = model.Id,
                    HolidayName = model.HolidayName,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    ModifiedDate = DateTime.Now
                };

                var result = baseService.ExecuteNonQuery(criteria);

                ViewBag.Updated = result.IsSuccessful;

                if (!result.IsSuccessful)
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

        [HRAuthorize(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Create()
        {
            var holiday = new Holiday
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };

            return View(holiday);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Create(Holiday model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertHolidayCriteria
                {
                    HolidayName = model.HolidayName,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    ModifiedDate = DateTime.Now
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                ViewBag.Updated = result.IsSuccessful;

                if (!result.IsSuccessful)
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

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Holiday)]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteHolidayCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}