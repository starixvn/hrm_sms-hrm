// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// LateInEarlyOut Controller.
    /// </summary>
    [Authorize]
    public class LateInEarlyOutController : RequestController
    {
        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="suggestedDate">The suggested date.</param>
        /// <param name="isLateIn">The is late in.</param>
        /// <returns>
        /// View Create.
        /// </returns>
        public ActionResult Create(DateTime? suggestedDate, bool? isLateIn)
        {
            var id = this.CurrentUser.Id;
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var listUnavailDates = _EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            //// Count number of Late in/early out request in current month.
            var currentDate = DateTime.Now.Date;
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var requestInMonthCriteria = new GetNumberRequestInMonthCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                RequestType = (int)RequestFilter.LateInEarlyOut,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = _RequestService.GetNumberRequestInMonth(requestInMonthCriteria);
            var viewModel = new LateInEarlyOutRequestViewModel()
            {
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Date = suggestedDate.HasValue ? suggestedDate.Value : DateTime.Today,
                Minute = 60,
                Type = (!isLateIn.HasValue || (isLateIn.HasValue && isLateIn.Value)) ? (short)RequestType.LateIn : (short)RequestType.EarlyOut,
                By = id
            };

            GetVerifierApproverList(id, viewModel);

            ViewBag.UnavailDates = listUnavailDates;
            ViewBag.TotalRequest = numberRequests;
            return View(viewModel);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        public ActionResult Create(LateInEarlyOutRequestViewModel model)
        {
            CheckIsTopManager(model);
            var id = this.CurrentUser.Id;
            model.By = id;
            if (string.Compare(model.Employee, model.DisplayName, true) != 0)
            {
                model.DisplayName = model.Employee;
            }

            ViewBag.Year = model.FromDate.Year;
            GetVerifierApproverList(id, model);
            if (ModelState.IsValid)
            {
                ViewBag.Requested = _LateInEarlyOutRequestService.Insert(ModelState, model);
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
            var listUnavailDates = _EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            ViewBag.UnavailDates = listUnavailDates;

            var request = _LateInEarlyOutRequestService.GetRequestById(id);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.EmpId))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = request.EmpId };
            var employee = _EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
            int minute = request.TotalMins;
            var type = short.Parse(request.RequestType.Id.ToString());

            var viewModel = new LateInEarlyOutRequestViewModel()
            {
                Id = request.Id,
                Employee = employee.NickName,
                DisplayName = employee.DisplayName,
                EmployeeId = employee.Id,
                Date = request.RequestedDate,
                Minute = minute,
                Type = type,
                By = this.CurrentUser.Id,
                Remark = request.Remark,
                ApproverId = request.ApprovedBy.Id,
                VerifierId = request.VerifiedBy.Id,
            };
            GetVerifierApproverList(this.CurrentUser.Id, viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        /// <returns>View after updated.</returns>
        [HttpPost]
        public ActionResult Update(LateInEarlyOutRequestViewModel model)
        {
            CheckIsTopManager(model);
            var id = this.CurrentUser.Id;
            model.By = id;
            GetVerifierApproverList(id, model);
            ViewBag.Year = model.FromDate.Year;
            if (ModelState.IsValid)
            {
                ViewBag.Updated = _LateInEarlyOutRequestService.Update(ModelState, model);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        /// <summary>
        /// ActionResult EmployeeView.
        /// </summary>
        /// <param name="year">Inputted Year.</param>
        /// <returns>View GetLateInEarlyOut of Employee in the Year.</returns>
        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            IList<RequestEntity> entityList = _RequestService.GetRequests(
                RequestViewType.CompanySpace,
                (int)RequestFilter.LateInEarlyOut,
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
        /// Gets the verifier approver list.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="viewModel">The view model.</param>
        private void GetVerifierApproverList(int id, LateInEarlyOutRequestViewModel viewModel)
        {
            var verifierList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Verifier, (byte)RequestType.LateIn);
            var approverList = this.GetSupervisorByRequestType(id, (byte)RoleSystem.Approver, (byte)RequestType.LateIn);
            viewModel.Verifier = verifierList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
            viewModel.Approver = approverList.Select(x => new SelectListItem { Text = x.NickName + " (" + x.FullName + ")", Value = x.OwnerId.ToString() }).ToList();
        }

        /// <summary>
        /// Check User is top manager or not.
        /// </summary>
        /// <param name="model">LateInEarlyOutRequest ViewModel.</param>
        private void CheckIsTopManager(LateInEarlyOutRequestViewModel model)
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
