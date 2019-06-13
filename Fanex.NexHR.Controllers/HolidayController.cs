namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;

        public HolidayController(
            ISessionService sessionService,
            IHolidayService holidayService) : base(sessionService)
        {
            this.holidayService = holidayService;
        }

        [HttpGet]
        [HRMPermission(Policy.Read, FunctionKey.Holiday)]
        public ActionResult Index(int? year)
        {
            int yearToCheck = year ?? DateTime.Now.Year;
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Host);
            ViewBag.Year = yearToCheck;

            IList<Holiday> holidays = holidayService.GetByYear(yearToCheck);

            return View(holidays);
        }

        [HRMPermission(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Edit(int id)
        {
            Holiday holiday = holidayService.GetById(id);

            return View(holiday);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Edit(Holiday model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = holidayService.Edit(sessionService.Employee.Id, model);
                ViewBag.Updated = commandResult.IsSuccessful;

                if (!commandResult.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.Holiday)]
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
        [HRMPermission(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Create(Holiday model)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = holidayService.Create(sessionService.Employee.Id, model);
                ViewBag.Updated = commandResult.IsSuccessful;

                if (!commandResult.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Holiday)]
        public JsonResult Delete(int id)
        {
            CommandResult commandResult = holidayService.Delete(sessionService.Employee.Id, id);

            return Json(commandResult);
        }
    }
}