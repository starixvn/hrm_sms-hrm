namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Shifts;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class ShiftController : BaseController
    {
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

        public ActionResult Index(int? statusId = 1)
        {
            var model = new ShiftByStatusViewModel
            {
                StatusId = statusId.Value
            };
            IList<Shift> shifts = shiftService.GetAll(statusId.Value);
            model.Shifts = shifts;
            return View(model);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(FunctionKey = FunctionKey.ShiftManagement, Policy = Policy.Create)]
        public ActionResult Create()
        {
            var model = new ShiftViewModel();
            ViewBag.Successed = false;
            return View(model);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        [HRAuthorize(FunctionKey = FunctionKey.ShiftManagement, Policy = Policy.Create)]
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
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(FunctionKey = FunctionKey.ShiftManagement, Policy = Policy.Edit)]
        public ActionResult Edit(int shiftId)
        {
            var criteria = new GetShiftByIdCriteria()
            {
                ShiftId = shiftId
            };
            var shift = shiftService.GetById(criteria);
            var model = new ShiftViewModel 
            {
                Name = shift.Name,
                HourTimeIn = GetHour(shift.BeginCheckInTime),
                MinuteTimeIn = GetMinute(shift.BeginCheckInTime),
                HourTimeOut = GetHour(shift.BeginCheckOutTime),
                MinuteTimeOut = GetMinute(shift.BeginCheckOutTime),
                IsDisabled = shift.IsDisabled,
                ShiftId = shift.Id,
                IsInAfterDay = shift.IsInAfterDay,
                IsOutAfterDay = shift.IsOutAfterDay,
                IsUsed = shift.IsUsed
            };
            ViewBag.Successed = false;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(FunctionKey = FunctionKey.ShiftManagement, Policy = Policy.Edit)]
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
                    ModifiedBy = this.CurrentUser.Id,
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
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        [HRAuthorize(FunctionKey = FunctionKey.ShiftManagement, Policy = Policy.Delete)]
        public bool Delete(int shiftId)
        {
            var criteria = new DeleteShiftCriteria()
            {
                ShiftId = shiftId
            };
            CommandResult result = commandService.Delete(criteria);
            return result.IsSuccessful;
        }

        private int GetHour(string time)
        {
            var hour = 0;
            if (!string.IsNullOrWhiteSpace(time))
            {
                var timeArr = time.Split(':');
                if (timeArr.Length == 2)
                {
                    hour = int.Parse(timeArr[0]);
                }
            }

            return hour;
        }

        private int GetMinute(string time)
        {
            var minute = 0;
            if (!string.IsNullOrWhiteSpace(time))
            {
                var timeArr = time.Split(':');
                if (timeArr.Length == 2)
                {
                    minute = int.Parse(timeArr[1]);
                }
            }

            return minute;
        }
    }
}
