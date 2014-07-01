// --------------------------------------------------------------------------
// <copyright file="ShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Shift Controller.
    /// </summary>
    [Authorize]
    public class ShiftController : BaseController
    {
        private readonly IShiftService _shiftService;

        public ShiftController()
            : this(null)
        {
        }

        public ShiftController(IShiftService shiftService = null)
        {
            _shiftService = shiftService ?? new ShiftService();
        }

        public ActionResult Index()
        {
            var shifts = _shiftService.GetAll(isShowAll: true);
            return View(shifts);
        }

        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Create()
        {
            var model = new ShiftViewModel();
            ViewBag.Successed = false;
            return View(model);
        }

        /// <summary>
        /// Action Create.
        /// </summary>
        /// <param name="model">Shift ViewModel.</param>
        /// <returns>View Add new.</returns>
        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public ActionResult Create(ShiftViewModel model)
        {
            var successed = false; 
            if (ModelState.IsValid)
            {
                var errorCode = 0;
                var message = string.Empty;
                var shift = new Shift 
                {
                    Name = model.Name,
                    BeginCheckInTime = model.HourTimeIn.ToString().PadLeft(2, '0') + ":" + model.MuniteTimeIn.ToString().PadLeft(2, '0'),
                    BeginCheckOutTime = model.HourTimeOut.ToString().PadLeft(2, '0') + ":" + model.MuniteTimeOut.ToString().PadLeft(2, '0'),
                    IsInAfterDay = model.IsInAfterDay,
                    IsOutAfterDay = model.IsOutAfterDay,
                    CreatedBy = this.CurrentUser.Id,
                    CreatedDate = DateTime.Now
                };

                _shiftService.Insert(shift, out errorCode, out message);

                if (errorCode != 0)
                {
                    ModelState.AddModelError(string.Empty, message);
                }
                else
                {
                    successed = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            ViewBag.Successed = successed;
            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="shiftId">The Shift Id.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Edit(int shiftId)
        {
            var shift = _shiftService.GetById(shiftId);
            var model = new ShiftViewModel 
            {
                Name = shift.Name,
                HourTimeIn = GetHour(shift.BeginCheckInTime),
                MuniteTimeIn = GetMinute(shift.BeginCheckInTime),
                HourTimeOut = GetHour(shift.BeginCheckOutTime),
                MuniteTimeOut = GetMinute(shift.BeginCheckOutTime),
                IsDisabled = shift.IsDisabled,
                ShiftId = shift.ShiftID,
                IsInAfterDay = shift.IsInAfterDay,
                IsOutAfterDay = shift.IsOutAfterDay,
                IsUsed = shift.IsUsed
            };
            ViewBag.Successed = false;

            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Team ViewModel.</param>
        /// <returns>View Edit.</returns>
        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Edit(ShiftViewModel model)
        {
            var successed = false;
            if (ModelState.IsValid)
            {
                var errorCode = 0;
                var message = string.Empty;
                var shift = new Shift
                {
                    ShiftID = model.ShiftId,
                    Name = model.Name,
                    BeginCheckInTime = model.HourTimeIn.ToString().PadLeft(2, '0') + ":" + model.MuniteTimeIn.ToString().PadLeft(2, '0'),
                    BeginCheckOutTime = model.HourTimeOut.ToString().PadLeft(2, '0') + ":" + model.MuniteTimeOut.ToString().PadLeft(2, '0'),
                    IsInAfterDay = model.IsInAfterDay,
                    IsOutAfterDay = model.IsOutAfterDay,
                    ModifiedBy = this.CurrentUser.Id,
                    ModifiedDate = DateTime.Now,
                    IsDisabled = model.IsDisabled
                };

                var result = _shiftService.Update(shift, out errorCode, out message);

                if (errorCode != 0)
                {
                    ModelState.AddModelError(string.Empty, message);
                }
                else
                {
                    successed = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            ViewBag.Successed = successed;
            return View(model);
        }

        /// <summary>
        /// Function to get hour of a text.
        /// </summary>
        /// <param name="time">The time (HH:mm).</param>
        /// <returns>Hour of time.</returns>
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

        /// <summary>
        /// Function to get minute of a text.
        /// </summary>
        /// <param name="time">The time (HH:mm).</param>
        /// <returns>Minute of time.</returns>
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
