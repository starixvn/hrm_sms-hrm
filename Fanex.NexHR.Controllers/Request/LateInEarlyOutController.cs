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
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels.Request;

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
            var currentDate = DateTime.Now.Date;
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var listUnavailDates = EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            //// Count number of Late in/early out request in current month.
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var requestInMonthCriteria = new GetNumberRequestInMonthCriteria()
            {
                EmployeeId = this.CurrentUser.Id,
                RequestType = (int)RequestType.LateIn,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = RequestService.GetNumberRequestInMonth(requestInMonthCriteria);
            var viewModel = new LateInEarlyOutRequestViewModel()
            {
                Employee = this.CurrentUser.NickName,
                EmployeeId = this.CurrentUser.Id,
                Date = suggestedDate.HasValue ? suggestedDate.Value : DateTime.Today,
                Minute = 60,
                RequestType = (!isLateIn.HasValue || (isLateIn.HasValue && isLateIn.Value)) ? (short)RequestType.LateIn : (short)RequestType.EarlyOut,
            };

            GetVerifierApproverList(this.CurrentUser.Id, viewModel);
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
            var criteria = new InsertLateInEarlyOutRequestCriteria()
            {
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                RequestType = model.RequestType,
                Remark = model.Remark,
                LateInMins = model.RequestType == (short)RequestType.LateIn ? model.Minute : 0,
                EarlyOutMins = model.RequestType == (short)RequestType.EarlyOut ? model.Minute : 0,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                CreatedBy = this.CurrentUser.Id,
                CreatedDate = DateTime.Now
            };
            var result = new CommandResult();
            if (ModelState.IsValid)
            {
                result = LateInEarlyOutRequestService.Insert(criteria);
                ViewBag.Requested = result.Code == 0;

                if (result.Code != 0)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            ViewBag.Year = model.Date.Year;
            GetVerifierApproverList(model.EmployeeId, model);

            return View(model);
        }

        /// <summary>
        /// ActionResult Update.
        /// </summary>
        /// <param name="id">WorkingTimeRequest id.</param>
        /// <returns>View Update.</returns>
        public ActionResult Update(int id)
        {
            var criteria = new GetRequestByIdCriteria()
            {
                RequestId = id
            };
            var request = RequestService.GetRequestById(criteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(this.CurrentUser.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            int minute = request.TotalMins;
            var type = short.Parse(request.RequestType.Id.ToString());

            var viewModel = new LateInEarlyOutRequestViewModel()
            {
                RequestId = request.Id,
                Employee = this.CurrentUser.NickName,
                EmployeeId = this.CurrentUser.Id,
                Date = request.RequestedDate,
                Minute = minute,
                RequestType = type,
                Remark = request.Remark,
                ApproverId = request.ApprovedBy.Id,
                VerifierId = request.VerifiedBy.Id
            };
            var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var listUnavailDates = EmployeeService.GetUnavailableDates(getUnavailableDateCriteria);
            ViewBag.UnavailDates = listUnavailDates;
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
            var criteria = new UpdateLateInEarlyOutRequestCriteria()
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                RequestType = model.RequestType,
                Remark = model.Remark,
                LateInMins = model.RequestType == (short)RequestType.LateIn ? model.Minute : 0,
                EarlyOutMins = model.RequestType == (short)RequestType.EarlyOut ? model.Minute : 0,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                CommandResult result = LateInEarlyOutRequestService.Update(criteria);
                ViewBag.Updated = result.Code == 0;

                if (result.Code != 0)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            GetVerifierApproverList(model.EmployeeId, model);
            ViewBag.Year = model.Date.Year;

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

            var criteria = new GetRequestCriteria()
            {
                EmpId = CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.LateInEarlyOut,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = CurrentUser.Id,
                IsLocal = null
            };

            IList<RequestEntity> entityList = RequestService.GetRequests(criteria);

            return View(entityList);
        }

        public ActionResult GetActualLateInEarlyOut(int employeeId, DateTime workDate)
        {
            var criteria = new GetActualLateInEarlyOutCriteria()
            {
                EmployeeId = employeeId,
                WorkDate = workDate
            };

            EmployeeLateInEarlyOutModel lateInEarlyOut = LateInEarlyOutRequestService.GetActualLateInEarlyOut(criteria);
            lateInEarlyOut = lateInEarlyOut == null ? new EmployeeLateInEarlyOutModel() : lateInEarlyOut;

            return Json(new { lateInEarlyOut.LateIn, lateInEarlyOut.EarlyOut }, JsonRequestBehavior.AllowGet);
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
            var employee = EmployeeService.GetEmployeeById(getEmployeeByIdCriteria);
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
