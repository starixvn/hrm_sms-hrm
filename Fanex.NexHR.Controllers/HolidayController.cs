namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    public class HolidayController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IHolidayService holidayService;

        public HolidayController(
            IUserControlSession userControlSession,
            IHolidayService holidayService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.holidayService = holidayService;
        }

        [HttpGet]
        [HRMPermission(Policy.Read, FunctionKey.Holiday)]
        public ActionResult Index(int? year)
        {
            int yearToCheck = year ?? DateTime.Now.Year;
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Host);
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
                CommandResult commandResult = holidayService.Edit(EmployeeSession.Id, model);
                ViewBag.Updated = commandResult.IsSuccessful;

                if (!commandResult.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.Holiday)]
        public ActionResult Create()
        {
            var holiday = new Holiday
            {
                TypeId = (byte)HolidayType.Regular,
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
                CommandResult commandResult = holidayService.Create(EmployeeSession.Id, model);
                ViewBag.Updated = commandResult.IsSuccessful;

                if (!commandResult.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Holiday)]
        public JsonResult Delete(int id)
        {
            CommandResult commandResult = holidayService.Delete(EmployeeSession.Id, id);

            return Json(commandResult);
        }
    }
}