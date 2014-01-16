// --------------------------------------------------------------------------
// <copyright file="ChangeShiftController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// ChangeShift Controller.
    /// </summary>
    [Authorize]
    public class ChangeShiftController : RequestController
    {
        #region Ctor
        public ChangeShiftController()
            : base(null, null, null, null)
        {
        }
        #endregion

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="year">Input year.</param>
        /// <returns>View Employee according to year.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _ChangeShiftRequestService.GetChangeShiftRequests(
                ((short)RolesEnum.Member).ToString(),
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                true,
                this.CurrentUser.Id);

            return View(entityList);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var id = this.CurrentUser.Id;
            var listUnavailDates = GetUnavailableDateList(id);
            ViewBag.UnavailDates = listUnavailDates;
            var employee = _EmployeeService.GetEmployeeById(id);
            var shift = _ShiftService.GetByDate(employee.Id, DateTime.Now.Date);
            var model = new ChangeShiftViewModel
            {
                Employee = employee.NickName,
                EmployeeId = employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                DisplayName = employee.DisplayName,
                By = this.CurrentUser.Id,
                ShiftDetails = new List<ShiftDetailsViewModel>(),
            };
            GetVerifierApproverList(id, model);
            if (shift != null)
            {
                model.FromShift = (int)shift.Id;
                model.FromShiftName = shift.Name;
            }
            //// Count number of ChangeShift request in current month.
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int numberOfRequest = 0;
            IList<RequestEntity> entityList = _ChangeShiftRequestService.GetChangeShiftRequests(
                ((short)RolesEnum.Member).ToString(),
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                string.Empty,
                true,
                this.CurrentUser.Id);
            foreach (var request in entityList)
            {
                if (request.Status.Id != (byte)CurrentRequestStatus.Rejected)
                {
                    numberOfRequest++;
                }
            }

            ViewBag.TotalRequest = numberOfRequest;
            ViewBag.Shifts = _ShiftService.GetAll();
            return View(model);
        }

        /// <summary>
        /// Create a new change shift.
        /// </summary>
        /// <param name="shiftJson">The shift data.</param>
        /// <param name="shiftDetailJson">The shift detail data.</param>
        /// <returns>Result to check success.</returns>
        [HttpPost]
        public JsonResult Create(string shiftJson, string shiftDetailJson)
        {
            var shift = JsonConverter.JsonExtensionMethod.FromJson<ChangeShiftViewModel>(shiftJson);
            shift.ShiftDetails = JsonConverter.JsonExtensionMethod.FromJsonArray<ShiftDetailsViewModel>(shiftDetailJson);
            shift.By = this.CurrentUser.Id;
            var insert = _ChangeShiftRequestService.InsertChangeShiftRequest(ModelState, shift);

            if (insert == false)
            {
                foreach (var item in ModelState.Values)
                {
                    foreach (ModelError error in item.Errors)
                    {
                        ////ModelState.AddModelError(string.Empty, error.ErrorMessage);
                        return Json(new { success = false, errors = error.ErrorMessage });
                    }
                }
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">Working time request's id.</param>
        /// <returns>View update.</returns>
        public ActionResult Update(int id)
        {
            var listUnavailDates = _EmployeeService.GetUnavailableDates(this.CurrentUser.Id, DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
            ViewBag.UnavailDates = listUnavailDates;
            var shift = _ChangeShiftRequestService.GetChangeShiftRequestById(id);
            if (shift == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, shift.EmpId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var shiftDetails = _ChangeShiftRequestService.GetShiftDetailsById(id);
            var shifts = _ShiftService.GetAll();
            var employee = _EmployeeService.GetEmployeeById(shift.EmpId);
            ViewBag.Shift = shifts;
            var model = new ChangeShiftViewModel();
            if (shift != null && shiftDetails != null)
            {
                List<ShiftDetailsViewModel> lstShiftDeatails = new List<ShiftDetailsViewModel>();
                foreach (var item in shiftDetails)
                {
                    ShiftDetailsViewModel shiftDetail = new ShiftDetailsViewModel
                    {
                        Date = item.OnDate.ToString("MM/dd/yyyy"),
                        FromShift = item.FromShiftID,
                        ToShift = item.ToShiftID,
                        ScheduleStatus = item.FromShift,
                        ChangeToShiftCode = item.ToShift,
                        Remark = item.Remark,
                        ShiftId = item.ToShiftID,
                        HasData = true
                    };
                    lstShiftDeatails.Add(shiftDetail);
                }

                model = new ChangeShiftViewModel
                {
                    Employee = employee.NickName,
                    DisplayName = employee.DisplayName,
                    EmployeeId = employee.Id,
                    FromDate = shift.FromDate.Date,
                    ToDate = shift.ToDate.Date,
                    VerifierId = shift.VerifiedBy.Id,
                    ApproverId = shift.ApprovedBy.Id,
                    Remark = shift.Remark,
                    ShiftDetails = lstShiftDeatails
                };
            }

            GetVerifierApproverList(employee.Id, model);
            return View(model);
        }

        [HttpPost]
        public JsonResult Update(string shiftJson, string shiftDetailJson, int requestId)
        {
            var shift = JsonConverter.JsonExtensionMethod.FromJson<ChangeShiftViewModel>(shiftJson);
            shift.ShiftDetails = JsonConverter.JsonExtensionMethod.FromJsonArray<ShiftDetailsViewModel>(shiftDetailJson);
            shift.By = this.CurrentUser.Id;
            shift.Id = requestId;

            var update = _ChangeShiftRequestService.UpdateChangeShiftRequest(ModelState, shift);
            return Json(new { success = true });
        }

        /// <summary>
        /// Changes the shift details.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Change Shift To date.</param>
        /// <returns>Action Result.</returns>
        public ActionResult ChangeShiftDetails(int employeeId, DateTime fromDate, DateTime toDate)
        {
            List<ShiftDetailsViewModel> lstmodel = new List<ShiftDetailsViewModel>();
            var shifts = _ShiftService.GetAll();
            ViewBag.Shift = shifts;
            var dayDiff = 0;
            List<DateTime> listDateTime = new List<DateTime>();
            listDateTime.Add(fromDate);
            if (toDate >= fromDate)
            {
                dayDiff = toDate.Subtract(fromDate).Days;

                for (int i = 1; i <= dayDiff; i++)
                {
                    var days = fromDate.AddDays(i);
                    listDateTime.Add(days);
                }
            }

            var unavailableDate = _EmployeeService.GetUnavailableDates(employeeId, fromDate, toDate).ToList();
            listDateTime.RemoveAll(i => unavailableDate.Contains(i));
            foreach (var item in listDateTime)
            {
                var shift = _ShiftService.GetByDate(employeeId, item);
                var model = new ShiftDetailsViewModel
                {
                    Date = item.ToString("ddd, MMM d, yyyy"),
                    ////Date = String.Format("{0:ddd, MMM d, yyyy}", item),
                    Remark = string.Empty,
                    ScheduleStatus = shift.Name,
                    FromShift = shift.ShiftID,
                    HasData = false
                };
                lstmodel.Add(model);
            }

            return PartialView("ChangeShiftDetails", lstmodel);
        }

        /// <summary>
        /// Gets the verifier approver list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModel">The view model.</param>
        private void GetVerifierApproverList(int id, ChangeShiftViewModel viewModel)
        {
            var verifierList = _EmployeeService.GetListSupervisorName(id, (int)RolesEnum.Verifier, (int)RequestTypes.ChangeShift);
            var approverList = _EmployeeService.GetListSupervisorName(id, (int)RolesEnum.Approver, (int)RequestTypes.ChangeShift);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        private IEnumerable<DateTime> GetUnavailableDateList(int id)
        {
            var previousMonth = DateTime.Now.AddMonths(-1).Date;
            var nextMonth = DateTime.Now.AddMonths(1).Date;
            var unvailableFromDate = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var unavailableToDate = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
            var listUnavailDates = _EmployeeService.GetUnavailableDates(id, unvailableFromDate, unavailableToDate);
            return listUnavailDates;
        }
    }
}
