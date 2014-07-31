// --------------------------------------------------------------------------
// <copyright file="CheckInOutController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Apply StyleCop</change>
// </history>
namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// CheckInOut Controller.
    /// </summary>
    [Authorize]
    public class CheckInOutController : RequestController
    {
        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// Get check in/out request of a specific employee in a year.
        /// </summary>
        /// <param name="year">Current Year.</param>
        /// <returns>View Employee.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _RequestService.GetRequests(
                RequestViewType.CompanySpace,
                (int)RequestFilter.CheckInOut,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                string.Empty,
                0,
                true,
                this.CurrentUser.Id,
                null,
                null);

            return View(entityList);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="suggestedDate">The suggested date.</param>
        /// <param name="isMissingCheckIn">The is missing check in.</param>
        /// <returns>
        /// View Create.
        /// </returns>
        public ActionResult Create(DateTime? suggestedDate, bool? isMissingCheckIn)
        {
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var listUnavailDates = _EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = this.CurrentUser.NickName,
                DisplayName = this.CurrentUser.DisplayName,
                EmployeeId = this.CurrentUser.Id,
                ForDate = suggestedDate.HasValue ? suggestedDate.Value : DateTime.Today,
                AttendantDate = suggestedDate.HasValue ? suggestedDate.Value : DateTime.Today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute,
                RequestType = (!isMissingCheckIn.HasValue || (isMissingCheckIn.HasValue && isMissingCheckIn.Value)) ? (short)RequestType.CheckIn : (short)RequestType.CheckOut,
                By = this.CurrentUser.Id
            };
            GetVerifierApproverList(this.CurrentUser.Id, viewModel);
            var getShiftByDateCriteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                Date = DateTime.Now
            };
            var shift = _ShiftService.GetByDate(getShiftByDateCriteria);
            if (shift != null)
            {
                CheckShiftFromToFormat(viewModel, shift);
            }

            viewModel.Shift = (shift == null) ? string.Empty : shift.Name;
            var currentDate = DateTime.Now.Date;
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var requestInMonthCriteria = new GetNumberRequestInMonthCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                RequestType = (int)RequestFilter.CheckInOut,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = _RequestService.GetNumberRequestInMonth(requestInMonthCriteria);
            ViewBag.Shifts = _ShiftService.GetAll();
            ViewBag.UnavailDates = listUnavailDates;
            ViewBag.TotalRequest = numberRequests;

            return View(viewModel);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">CheckInOutRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(CheckInOutRequestViewModel model)
        {
            CheckIsTopManager(model);
            var id = this.CurrentUser.Id;
            GetVerifierApproverList(id, model);
            model.By = id;
            ViewBag.Shifts = _ShiftService.GetAll();
            ViewBag.Year = model.ForDate.Year;
            if (string.Compare(model.Employee, model.DisplayName, true) != 0)
            {
                model.DisplayName = model.Employee;
            }

            if (ModelState.IsValid)
            {
                ViewBag.Requested = _CheckInOutRequestService.Insert(ModelState, model);
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">WorkingTimeRequest id.</param>
        /// <returns>View Update.</returns>
        public ActionResult Update(int id)
        {
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var listUnavailableDates = _EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            ViewBag.UnavailDates = listUnavailableDates;

            ViewBag.Shifts = _ShiftService.GetAll();

            var request = _CheckInOutRequestService.GetRequestById(id);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.EmpId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var type = short.Parse(request.RequestType.Id.ToString());
            var getShiftByDateCriteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                Date = DateTime.Now
            };
            var shift = _ShiftService.GetByDate(getShiftByDateCriteria);

            var viewModel = new CheckInOutRequestViewModel()
            {
                Id = request.Id,
                Employee = this.CurrentUser.NickName,
                DisplayName = this.CurrentUser.DisplayName,
                EmployeeId = this.CurrentUser.Id,
                ForDate = request.RequestedDate.Date,
                Hour = request.RequestedDate.Hour,
                Minute = request.RequestedDate.Minute,
                RequestType = type,
                By = this.CurrentUser.Id,
                Shift = (shift == null) ? string.Empty : shift.Name,
                ApproverId = request.ApprovedBy.Id,
                VerifierId = request.VerifiedBy.Id,
                RefNO = request.RefNO,
                AttendantDate = request.FromDate,
                Remark = request.Remark
            };

            GetVerifierApproverList(this.CurrentUser.Id, viewModel);

            return View(viewModel);
        }

        /// <summary>
        /// Updates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Return to view.</returns>
        [HttpPost]
        public ActionResult Update(CheckInOutRequestViewModel model)
        {
            CheckIsTopManager(model);
            var id = this.CurrentUser.Id;
            model.By = id;
            ViewBag.Shifts = _ShiftService.GetAll();
            ViewBag.Year = model.ForDate.Year;
            GetVerifierApproverList(id, model);
            if (ModelState.IsValid)
            {
                ViewBag.Updated = _CheckInOutRequestService.Update(ModelState, model);
            }

            return View(model);
        }

        /// <summary>
        /// Gets the verifier approver list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModel">The view model.</param>
        private void GetVerifierApproverList(int id, CheckInOutRequestViewModel viewModel)
        {
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Verifier, (byte)RequestType.CheckIn);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Approver, (byte)RequestType.CheckIn);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        /// <summary>
        /// Checks the shift from to format.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="shift">The shift.</param>
        private void CheckShiftFromToFormat(CheckInOutRequestViewModel viewModel, Shift shift)
        {
            var checkContainColon = shift.BeginCheckInTime.Contains(":");
            if (checkContainColon)
            {
                var time = shift.BeginCheckInTime.Split(new[] { ':' });
                viewModel.Hour = int.Parse(time[0]);
                viewModel.Minute = int.Parse(time[1]);
            }
            else
            {
                viewModel.Hour = 0;
                viewModel.Minute = 0;
            }
        }

        /// <summary>
        /// Check request is come from TopManager.
        /// </summary>
        /// <param name="model">CheckInOutRequest ViewModel.</param>
        private void CheckIsTopManager(CheckInOutRequestViewModel model)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmployeeId };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            if (employee.IsTopManager)
            {
                foreach (var modelError in ModelState)
                {
                    string propertyName = modelError.Key;

                    if (propertyName.Contains("VerifierId") || propertyName.Contains("ApproverId"))
                    {
                        ModelState[propertyName].Errors.Clear();
                    }
                }
            }
        }
    }
}
