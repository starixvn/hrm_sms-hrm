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
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels.Request;

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
            var unavailableDates = EmployeeService.GetNotOffOrHolidayDate(criteria);
            var getShiftByDateCriteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                Date = DateTime.Now
            };
            var shift = ShiftService.GetByDate(getShiftByDateCriteria);

            var model = new KeepInViewRequestViewModel
            {
                Employee = this.CurrentUser.NickName,
                EmployeeId = this.CurrentUser.Id,
                Date = DateTime.Now.Date,
                Shift = (shift == null) ? string.Empty : shift.Name,
                ShiftId = (shift == null) ? 0 : shift.Id
            };

            GetVerifierApproverList(this.CurrentUser.Id, model);
            ViewBag.UnavailableDates = unavailableDates;
            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">KeepInViewRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(KeepInViewRequestViewModel model)
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);
            var getOffAndHolidaycriteria = new GetOffDateAndHolidayDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = fromDate,
                ToDate = toDate
            };
            var unavailableDates = EmployeeService.GetNotOffOrHolidayDate(getOffAndHolidaycriteria);
            var criteria = new InsertKeepInViewRequestCriteria()
            {
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                RequestType = (int)RequestType.KeepInView,
                RefNo = model.RefNo,
                Remark = model.Remark,
                IsHalfDay = model.HalfDay,
                ShiftId = model.ShiftId,
                ShiftName = model.Shift,
                FromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime),
                ToTime = model.CrossDay 
                ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime)
                : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime),
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                CreatedBy = this.CurrentUser.Id,
                CreatedDate = DateTime.Now
            };

            var result = new CommandResult();
            if (ModelState.IsValid)
            {
                result = KeepInViewRequestService.Insert(criteria);
                ViewBag.Requested = result.Code == 0;

                if (result.Code != 0)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            GetVerifierApproverList(this.CurrentUser.Id, model);
            ViewBag.Year = model.Date.Year;
            ViewBag.UnavailableDates = unavailableDates;
            return View(model);
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
            var unavailableDates = EmployeeService.GetNotOffOrHolidayDate(criteria);
            var getRequestByIdcriteria = new GetRequestByIdCriteria()
            {
                RequestId = id
            };
            var request = RequestService.GetRequestById(getRequestByIdcriteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var viewModel = new KeepInViewRequestViewModel()
            {
                RequestId = id,
                RefNo = request.RefNO,
                HalfDay = request.IsHalfDay,
                Employee = this.CurrentUser.NickName,
                EmployeeId = this.CurrentUser.Id,
                Date = request.RequestedDate,
                Shift = request.FromShift.Name,
                ShiftId = request.FromShift.Id,
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
        /// <param name="model">The keep in view request view model.</param>
        /// <returns>Return the update result to client.</returns>
        [HttpPost]
        public ActionResult Update(KeepInViewRequestViewModel model)
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);

            var getOffAndHolidaycriteria = new GetOffDateAndHolidayDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = fromDate,
                ToDate = toDate
            };
            var unavailableDates = EmployeeService.GetNotOffOrHolidayDate(getOffAndHolidaycriteria);

            var criteria = new UpdateKeepInViewRequestCriteria()
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                RequestType = (int)RequestType.KeepInView,
                RefNo = model.RefNo,
                Remark = model.Remark,
                IsHalfDay = model.HalfDay,
                ShiftId = model.ShiftId,
                ShiftName = model.Shift,
                FromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime),
                ToTime = model.CrossDay
                ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime)
                : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime),
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                CommandResult result = KeepInViewRequestService.Update(criteria);
                ViewBag.Updated = result.Code == 0;

                if (result.Code != 0)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            GetVerifierApproverList(this.CurrentUser.Id, model);
            ViewBag.Year = model.Date.Year;
            ViewBag.UnavailableDates = unavailableDates;
            return View(model);
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

            var criteria = new GetRequestCriteria()
            {
                EmpId = CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.KeepInView,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = CurrentUser.Id,
                IsLocal = null
            };

            IList<RequestEntity> entityList = RequestService.GetRequests(criteria);

            return View(entityList);
        }

        [NonAction]
        private void GetVerifierApproverList(int id, KeepInViewRequestViewModel viewModel)
        {
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Verifier, (byte)RequestType.KeepInView);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Approver, (byte)RequestType.KeepInView);
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
