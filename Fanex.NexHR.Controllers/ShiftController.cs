namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Shifts;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Web.Mvc;

    public class ShiftController : BaseController
    {
        private const int HourIndex = 0;
        private const int MinutesIndex = 1;
        private readonly IShiftService shiftService;
        private readonly ICommandService commandService;

        public ShiftController()
            : this(null, null)
        {
        }

        public ShiftController(ICommandService commandService, IShiftService shiftService)
        {
            this.commandService = commandService ?? new CommandService();
            this.shiftService = shiftService ?? new ShiftService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.ShiftManagement)]
        public ActionResult Index(int? statusId = (int)ShiftStatus.Active)
        {
            var model = new ShiftByStatusViewModel
            {
                StatusId = statusId.Value,
                Shifts = shiftService.GetAll(statusId.Value)
            };

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.ShiftManagement, Policy.Host);

            return View(model);
        }

        [HRAuthorize(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Create()
        {
            var model = new ShiftViewModel();
            ViewBag.Successed = false;
            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Create(ShiftViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertShiftCriteria()
                {
                    Name = model.Name,
                    CheckInTime = model.TimeIn,
                    CheckOutTime = model.TimeOut,
                    IsInAfterDay = model.IsInAfterDay,
                    IsOutAfterDay = model.IsOutAfterDay,
                    CreatedBy = CurrentUser.Id,
                    CreatedDate = DateTime.Now
                };

                CommandResult result = commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Successed = true;
                }
                else
                {
                    ViewBag.Successed = false;
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [HRAuthorize(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Edit(int shiftId)
        {
            var criteria = new GetShiftByIdCriteria() { ShiftId = shiftId };
            var shift = shiftService.GetById(criteria);
            var model = new ShiftViewModel
            {
                Name = shift.Name,
                HourTimeIn = GetTime(shift.BeginCheckInTime, HourIndex),
                MinuteTimeIn = GetTime(shift.BeginCheckInTime, MinutesIndex),
                HourTimeOut = GetTime(shift.BeginCheckOutTime, HourIndex),
                MinuteTimeOut = GetTime(shift.BeginCheckOutTime, MinutesIndex),
                IsDisabled = shift.IsDisabled,
                ShiftId = shift.Id,
                IsInAfterDay = shift.IsInAfterDay,
                IsOutAfterDay = shift.IsOutAfterDay,
                IsUsed = shift.IsUsed
            };

            ViewBag.Successed = false;
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.ShiftManagement, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ShiftManagement)]
        public ActionResult Edit(ShiftViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateShiftCriteria()
                {
                    ShiftId = model.ShiftId,
                    Name = model.Name,
                    CheckInTime = model.TimeIn,
                    CheckOutTime = model.TimeOut,
                    IsInAfterDay = model.IsInAfterDay,
                    IsOutAfterDay = model.IsOutAfterDay,
                    ModifiedBy = CurrentUser.Id,
                    ModifiedDate = DateTime.Now,
                    IsDisabled = model.IsDisabled
                };

                CommandResult result = commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Successed = true;
                }
                else
                {
                    ViewBag.Successed = false;
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.ShiftManagement, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ShiftManagement)]
        public JsonResult Delete(int shiftId)
        {
            var criteria = new DeleteShiftCriteria() { ShiftId = shiftId };
            CommandResult result = commandService.Delete(criteria);

            return Json(result);
        }

        private static int GetTime(string time, int index = 0)
        {
            var value = 0;
            if (!string.IsNullOrWhiteSpace(time))
            {
                var timeArr = time.Split(':');
                if (timeArr.Length == 2)
                {
                    value = int.Parse(timeArr[index]);
                }
            }

            return value;
        }
    }
}
