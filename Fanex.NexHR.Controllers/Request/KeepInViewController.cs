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
    using System.Diagnostics.CodeAnalysis;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// KeepInView Controller.
    /// </summary>
    [Authorize]
    public class KeepInViewController : RequestController
    {
        private const int _ONE_WEEK = 7;
        private const int _ONE_MONTH = 1;

        public KeepInViewController()
            : base()
        {
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        public ActionResult Create()
        {
            var fromDate = DateTime.Now.AddDays(-_ONE_WEEK);
            var toDate = DateTime.Now.AddMonths(_ONE_MONTH);
            var unavailableDates = _EmployeeService.GetOffDateAndHolidayDate(this.CurrentUser.Id, fromDate, toDate);
            var employee = _EmployeeService.GetEmployeeById(this.CurrentUser.Id);
            var verifiers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Verifier, (int)RequestTypes.KeepInView);
            var approvers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Approver, (int)RequestTypes.KeepInView);
            var shift = _ShiftService.GetByDate(employee.Id, DateTime.Now);

            var model = new KeepInViewRequestViewModel
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Date = DateTime.Now.Date,
                By = this.CurrentUser.Id,
                VerifierList = verifiers,
                ApproverList = approvers,
                Shift = (shift == null) ? string.Empty : shift.Name,
                ShiftId = (shift == null) ? 0 : shift.ShiftID
            };

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
            var currentUserId = this.CurrentUser.Id;
            var verifiers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Verifier, (int)RequestTypes.KeepInView);
            var approvers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Approver, (int)RequestTypes.KeepInView);
            keepInViewRequestViewModel.By = this.CurrentUser.Id;

            keepInViewRequestViewModel.VerifierList = verifiers;
            keepInViewRequestViewModel.ApproverList = approvers;

            ViewBag.Requested = _KeepInViewRequestService.InsertKeepInViewRequest(ModelState, keepInViewRequestViewModel, this.CurrentUser.IsTopManager);
            ViewBag.Year = keepInViewRequestViewModel.Date.Year;

            return View(keepInViewRequestViewModel);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">KeepInViewRequest id.</param>
        /// <returns>View Update.</returns>
        public ActionResult Update(long id)
        {
            var request = _KeepInViewRequestService.GetKeepInViewRequestById(id);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.EmpId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var employee = _EmployeeService.GetEmployeeById(request.EmpId);
            var verifiers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Verifier, (int)RequestTypes.KeepInView);
            var approverList = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Approver, (int)RequestTypes.KeepInView);

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
                VerifierList = verifiers,
                ApproverList = approverList,
                Shift = request.Shift,
                ShiftId = request.ShiftId,
                ApproverId = request.ApprovedBy.Id,
                VerifierId = request.VerifiedBy.Id,
                Remark = request.Remark,
                CrossDay = request.CrossDay,
                FromTime = request.FromTime,
                ToTime = request.ToTime
            };

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
            var verifiers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Verifier, (int)RequestTypes.KeepInView);
            var approvers = _EmployeeService.GetListSupervisorName(this.CurrentUser.Id, (int)RolesEnum.Approver, (int)RequestTypes.KeepInView);
            keepInViewRequestViewModel.By = this.CurrentUser.Id;

            keepInViewRequestViewModel.VerifierList = verifiers;
            keepInViewRequestViewModel.ApproverList = approvers;

            ViewBag.Updated = _KeepInViewRequestService.UpdateKeepInViewRequest(ModelState, keepInViewRequestViewModel, this.CurrentUser.IsTopManager);
            ViewBag.Year = keepInViewRequestViewModel.Date.Year;

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

            IList<RequestEntity> entityList = _KeepInViewRequestService.GetKeepInViewRequests(
                ((short)RolesEnum.Member).ToString(),
                fromDate,
                toDate,
                string.Empty,
                true,
                this.CurrentUser.Id);

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
            return _KeepInViewRequestService.DeleteKeepInViewRequestById(id);
        }
    }
}
