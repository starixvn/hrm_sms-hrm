namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Shifts;

    public class ShiftController : BaseController
    {
        private readonly IShiftService shiftService;

        public ShiftController(
            ISessionService sessionService,
            IShiftService shiftService) : base(sessionService)
        {
            this.shiftService = shiftService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ShiftManagement)]
        public ActionResult Index(int? statusId = (int)ShiftStatus.Active)
        {
            var model = new ShiftByStatusViewModel
            {
                StatusId = statusId.Value,
                Shifts = shiftService.GetAll(statusId.Value)
            };
            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Create()
        {
            var model = new CreateShiftModel();
            ViewBag.Successed = false;
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Create(CreateShiftModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                CommandResult commandResult = shiftService.Create(sessionService.Employee.Id, model);
                if (commandResult.IsSuccessful)
                {
                    return RedirectToAction("Index", "Shift");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Edit(int id)
        {
            const byte HourIndex = 0;
            const byte MinutesIndex = 1;
            Shift shift = shiftService.GetById(id);
            EditShiftModel model = null;

            if (shift != null)
            {
                model = new EditShiftModel
                {
                    ShiftId = shift.Id,
                    Name = shift.Name,
                    HourTimeIn = GetTime(shift.BeginCheckInTime, HourIndex),
                    MinuteTimeIn = GetTime(shift.BeginCheckInTime, MinutesIndex),
                    HourTimeOut = GetTime(shift.BeginCheckOutTime, HourIndex),
                    MinuteTimeOut = GetTime(shift.BeginCheckOutTime, MinutesIndex),
                    IsInAfterDay = shift.IsInAfterDay,
                    IsOutAfterDay = shift.IsOutAfterDay,
                    IsDisabled = shift.IsDisabled,
                    IsUsed = shift.IsUsed
                };
            }

            ViewBag.Successed = false;
            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Edit(EditShiftModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                CommandResult commandResult = shiftService.Edit(sessionService.Employee.Id, model);
                if (commandResult.IsSuccessful)
                {
                    return RedirectToAction("Index", "Shift");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        public JsonResult Delete(int shiftId)
        {
            CommandResult commandResult = shiftService.Delete(shiftId);
            return Json(commandResult);
        }

        public JsonResult GetShiftByDate(int employeeId, DateTime date)
        {
            Shift shift = shiftService.GetByDate(employeeId, date);
            return Json(shift, JsonRequestBehavior.AllowGet);
        }

        private static int GetTime(string time, int index)
        {
            var value = 0;
            if (!string.IsNullOrWhiteSpace(time))
            {
                var timeArr = time.Split(':');
                const byte TotalItems = 2;
                if (timeArr.Length == TotalItems)
                {
                    value = int.Parse(timeArr[index]);
                }
            }

            return value;
        }
    }
}