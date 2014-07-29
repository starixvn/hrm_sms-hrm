// --------------------------------------------------------------------------
// <copyright file="KeepInViewController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.11">Created</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.MySpace;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// KeepInView Controller.
    /// </summary>
    [Authorize]
    public class KeepInViewController : RequestController
    {
        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);

            var criteria = new GetOffDateAndHolidayDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = fromDate,
                ToDate = toDate
            };
            var unavailableDates = _EmployeeService.GetOffDateAndHolidayDate(criteria);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = this.CurrentUser.Id };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var getShiftByDateCriteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                Date = DateTime.Now
            };
            var shift = _ShiftService.GetByDate(getShiftByDateCriteria);

            var model = new KeepInViewRequestViewModel
            {
                Employee = this.CurrentUser.NickName,
                DisplayName = this.CurrentUser.DisplayName,
                EmployeeId = this.CurrentUser.Id,
                Date = DateTime.Now.Date,
                By = this.CurrentUser.Id,
                Shift = (shift == null) ? string.Empty : shift.Name,
                ShiftId = (shift == null) ? 0 : shift.ShiftID
            };

            GetVerifierApproverList(this.CurrentUser.Id, model);

            ViewBag.UnavailableDates = unavailableDates;
            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="keepInViewRequestViewModel">KeepInViewRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(KeepInViewRequestViewModel keepInViewRequestViewModel)
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);

            var criteria = new GetOffDateAndHolidayDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = fromDate,
                ToDate = toDate
            };
            var unavailableDates = _EmployeeService.GetOffDateAndHolidayDate(criteria);

            keepInViewRequestViewModel.By = this.CurrentUser.Id;

            GetVerifierApproverList(this.CurrentUser.Id, keepInViewRequestViewModel);

            ViewBag.Requested = _KeepInViewRequestService.Insert(ModelState, keepInViewRequestViewModel, this.CurrentUser.IsTopManager);
            ViewBag.Year = keepInViewRequestViewModel.Date.Year;
            ViewBag.UnavailableDates = unavailableDates;

            return View(keepInViewRequestViewModel);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">KeepInViewRequest id.</param>
        /// <returns>View Update.</returns>
        public ActionResult Update(long id)
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);

            var criteria = new GetOffDateAndHolidayDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = fromDate,
                ToDate = toDate
            };
            var unavailableDates = _EmployeeService.GetOffDateAndHolidayDate(criteria);

            var request = _KeepInViewRequestService.GetRequestById(id);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.EmpId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = request.EmpId };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var viewModel = new KeepInViewRequestViewModel()
            {
                Id = request.Id,
                RefNo = request.RefNO,
                HalfDay = request.IsHalfDay,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Date = request.RequestedDate,
                By = this.CurrentUser.Id,
                Shift = request.Shift.Name,
                ShiftId = request.Shift.Id,
                ApproverId = request.ApprovedBy.Id,
                VerifierId = request.VerifiedBy.Id,
                Remark = request.Remark,
                CrossDay = request.CrossDay,
                FromTime = request.FromTime.ToString("HH:mm"),
                ToTime = request.ToTime.ToString("HH:mm")
            };

            GetVerifierApproverList(this.CurrentUser.Id, viewModel);

            ViewBag.UnavailableDates = unavailableDates;
            return View(viewModel);
        }

        /// <summary>
        /// Updates the specified keep in view request view model.
        /// </summary>
        /// <param name="keepInViewRequestViewModel">The keep in view request view model.</param>
        /// <returns>Return the update result to client.</returns>
        [HttpPost]
        public ActionResult Update(KeepInViewRequestViewModel keepInViewRequestViewModel)
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);

            var criteria = new GetOffDateAndHolidayDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = fromDate,
                ToDate = toDate
            };
            var unavailableDates = _EmployeeService.GetOffDateAndHolidayDate(criteria);

            GetVerifierApproverList(this.CurrentUser.Id, keepInViewRequestViewModel);
            keepInViewRequestViewModel.By = this.CurrentUser.Id;

            ViewBag.Updated = _KeepInViewRequestService.Update(ModelState, keepInViewRequestViewModel, this.CurrentUser.IsTopManager);
            ViewBag.Year = keepInViewRequestViewModel.Date.Year;
            ViewBag.UnavailableDates = unavailableDates;

            return View(keepInViewRequestViewModel);
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// Employees the view.
        /// </summary>
        /// <param name="year">The year is used for filtering.</param>
        /// <returns>The view with filtered by the year.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;
            var fromDate = new DateTime(year, 1, 1);
            var toDate = new DateTime(year, 12, 31);

            IList<RequestEntity> entityList = _RequestService.GetRequests(
                RequestViewType.CompanySpace,
                (int)RequestFilter.KeepInView,
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
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Indicate the delete action is successful or not.</returns>
        [HttpPost]
        public bool Delete(long id)
        {
            return _RequestService.DeleteWorkingTimeRequest(id);
        }

        [NonAction]
        private void GetVerifierApproverList(int id, KeepInViewRequestViewModel viewModel)
        {
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RolesEnum.Verifier, (byte)RequestTypes.KeepInView);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RolesEnum.Approver, (byte)RequestTypes.KeepInView);
            viewModel.VerifierList = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.ApproverList = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        [NonAction]
        private IList<SelectListItem> GetListItem(IList<SupervisorNameModel> supervisors)
        {
            IList<SelectListItem> selectItems = supervisors.Select(x => new SelectListItem
            {
                Text = x.NickName + " (" + x.FullName + ")",
                Value = x.OwnerId.ToString()
            }).ToList();

            return selectItems;
        }
    }
}
