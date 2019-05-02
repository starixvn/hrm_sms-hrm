namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class LateInEarlyOutController : BaseController
    {
        private readonly ILateInEarlyOutRequestService lateInEarlyOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public LateInEarlyOutController(
            ILateInEarlyOutRequestService lateInEarlyOutRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IEmployeeService employeeService)
        {
            this.lateInEarlyOutRequestService = lateInEarlyOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        public ActionResult Create(DateTime? suggestedDate, bool? isLateIn)
        {
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                Employee = CurrentUser.NickName,
                EmployeeId = CurrentUser.Id,
                Date = suggestedDate.HasValue ? suggestedDate.Value : DateTime.Today,
                Minute = 60,
                RequestType = (isLateIn.HasValue && isLateIn.Value) ? (short)RequestType.LateIn : (short)RequestType.EarlyOut,
                UnavailDates = GetUnavailDate(CurrentUser.Id),
                VerifierAndApprover = GetVerifierApprover(CurrentUser.Id),
                TotalRequestInMonth = GetTotalRequestInMonth(CurrentUser.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(LateInEarlyOutRequestViewModel model)
        {
            if (model != null)
            {
                CheckIsTopManager(model);
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);
                model.TotalRequestInMonth = GetTotalRequestInMonth(model.EmployeeId);
                model.UnavailDates = GetUnavailDate(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new InsertLateInEarlyOutRequestCriteria
                    {
                        EmpId = model.EmployeeId,
                        OnDate = model.Date,
                        RequestType = model.RequestType,
                        Remark = model.Remark,
                        LateInMins = model.RequestType == (short)RequestType.LateIn ? model.Minute : 0,
                        EarlyOutMins = model.RequestType == (short)RequestType.EarlyOut ? model.Minute : 0,
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        CreatedBy = CurrentUser.Id
                    };
                    CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);

                    if (result.IsSuccessful)
                    {
                        lateInEarlyOutRequestService.SendMailForInsertSuccessfully(criteria);
                        ViewBag.Requested = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View(model);
        }

        public ActionResult Update(int id)
        {
            var criteria = new GetRequestByIdCriteria
            {
                RequestId = id
            };
            RequestEntity request = requestService.GetRequestById(criteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(CurrentUser.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            int minute = request.TotalMins;
            var type = short.Parse(request.RequestType.Id.ToString());

            var viewModel = new LateInEarlyOutRequestViewModel
            {
                RequestId = request.Id,
                Employee = CurrentUser.NickName,
                EmployeeId = CurrentUser.Id,
                Date = request.RequestedDate,
                Minute = minute,
                RequestType = type,
                Remark = request.Remark,
                ApproverId = request.ApprovedBy.Id,
                VerifierId = request.VerifiedBy.Id,
                UnavailDates = GetUnavailDate(CurrentUser.Id),
                VerifierAndApprover = GetVerifierApprover(CurrentUser.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(LateInEarlyOutRequestViewModel model)
        {
            if (model != null)
            {
                CheckIsTopManager(model);
                model.VerifierAndApprover = GetVerifierApprover(model.EmployeeId);
                model.TotalRequestInMonth = GetTotalRequestInMonth(model.EmployeeId);
                model.UnavailDates = GetUnavailDate(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new UpdateLateInEarlyOutRequestCriteria
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
                        ModifiedBy = CurrentUser.Id,
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria);
                    if (result.IsSuccessful)
                    {
                        lateInEarlyOutRequestService.SendMailForUpdateSuccessfully(criteria);
                        ViewBag.Updated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            return EmployeeView(DateTime.Now.Year);
        }

        [HttpPost]
        public ActionResult EmployeeView(int year)
        {
            ViewBag.Year = year;

            var criteria = new GetRequestCriteria
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

            IList<RequestEntity> entityList = requestService.GetRequests(criteria);

            return View(entityList);
        }

        public ActionResult GetActualLateInEarlyOut(int employeeId, DateTime workDate)
        {
            var criteria = new GetActualLateInEarlyOutCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate
            };

            EmployeeLateInEarlyOutModel lateInEarlyOut = lateInEarlyOutRequestService.GetActualLateInEarlyOut(criteria);
            lateInEarlyOut = lateInEarlyOut ?? new EmployeeLateInEarlyOutModel();

            return Json(new { lateInEarlyOut.LateIn, lateInEarlyOut.EarlyOut }, JsonRequestBehavior.AllowGet);
        }

        private IList<DateTime> GetUnavailDate(int employeeId)
        {
            var criteria = new GetUnavailableRequestDateCriteria
            {
                EmpId = employeeId,
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now.AddMonths(1)
            };
            var unavailDates = employeeService.GetUnavailableDates(criteria);
            return unavailDates;
        }

        private decimal GetTotalRequestInMonth(int employeeId)
        {
            var currentDate = DateTime.Now.Date;
            var criteria = new GetNumberRequestInMonthCriteria
            {
                EmployeeId = employeeId,
                RequestType = (int)RequestType.LateIn,
                FromDate = new DateTime(currentDate.Year, currentDate.Month, 1),
                ToDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
            };
            decimal numberRequests = requestService.GetNumberRequestInMonth(criteria);
            return numberRequests;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId)
        {
            var criteria = new GetVerifierAndApproverCriteria
            {
                EmpId = employeeId,
                RequestType = (byte)RequestType.LateIn
            };
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(criteria);
            return supervisor;
        }

        private void CheckIsTopManager(LateInEarlyOutRequestViewModel model)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria { EmpId = model.EmployeeId };
            var employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
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