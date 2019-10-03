namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class LateInEarlyOutController : BaseController
    {
        private const int DefaultMinutes = 60;
        private readonly ILateInEarlyOutRequestService lateInEarlyOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public LateInEarlyOutController(
            ISessionService sessionService,
            ILateInEarlyOutRequestService lateInEarlyOutRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.lateInEarlyOutRequestService = lateInEarlyOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            ViewBag.Year = tempYear;
            var criteria = new GetLateInEarlyOutRequestByDateCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(tempYear, 1, 1),
                ToDate = new DateTime(tempYear, 12, 31)
            };
            IList<LateInEarlyOutRequest> requests = lateInEarlyOutRequestService.GetByDate(criteria);
            return View(requests);
        }

        public ActionResult Create(DateTime? suggestedDate, bool? isLateIn)
        {
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                Employee = sessionService.Employee.Nickname,
                EmployeeId = sessionService.Employee.Id,
                OnDate = suggestedDate ?? DateTime.Today,
                Minute = DefaultMinutes,
                ReasonId = (byte)LateInEarlyOutReasons.Other,
                RequestType = (isLateIn.HasValue && !isLateIn.Value) ? (int)RequestType.EarlyOut : (int)RequestType.LateIn,
                UnavailDates = GetUnavailDate(sessionService.Employee.Id),
                VerifierAndApprover = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.LateIn),
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(LateInEarlyOutRequestViewModel model)
        {
            if (model != null)
            {
                CheckIsTopManager(model);
                model.VerifierAndApprover = requestService.GetVerifierAndApprover(model.EmployeeId, RequestType.LateIn);
                model.UnavailDates = GetUnavailDate(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new InsertLateInEarlyOutRequestCriteria
                    {
                        EmpId = model.EmployeeId,
                        OnDate = model.OnDate,
                        RequestType = model.RequestType,
                        Remark = model.Remark,
                        LateInMins = model.RequestType == (int)RequestType.LateIn ? model.Minute : 0,
                        EarlyOutMins = model.RequestType == (int)RequestType.EarlyOut ? model.Minute : 0,
                        ReasonId = model.ReasonId,
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        CreatedBy = sessionService.Employee.Id
                    };
                    CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);

                    if (result.IsSuccessful)
                    {
                        lateInEarlyOutRequestService.SendMailCreateRequest(result.OutputId);
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
            LateInEarlyOutRequest request = lateInEarlyOutRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Employee.Id)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            var minutes = request.RequestTypeId == (int)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins;
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                RequestId = request.RequestId,
                Employee = sessionService.Employee.Nickname,
                EmployeeId = sessionService.Employee.Id,
                OnDate = request.OnDate,
                Minute = minutes,
                RequestType = request.RequestTypeId,
                Remark = request.Remark,
                ReasonId = request.ReasonId,
                ApproverId = request.ApprovedBy.Id,
                VerifierId = request.VerifiedBy.Id,
                UnavailDates = GetUnavailDate(sessionService.Employee.Id),
                VerifierAndApprover = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.LateIn)
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Update(LateInEarlyOutRequestViewModel model)
        {
            if (model != null)
            {
                CheckIsTopManager(model);
                model.VerifierAndApprover = requestService.GetVerifierAndApprover(model.EmployeeId, RequestType.LateIn);
                model.UnavailDates = GetUnavailDate(model.EmployeeId);

                if (ModelState.IsValid)
                {
                    var criteria = new UpdateLateInEarlyOutRequestCriteria
                    {
                        RequestId = model.RequestId,
                        EmpId = model.EmployeeId,
                        OnDate = model.OnDate,
                        RequestType = model.RequestType,
                        Remark = model.Remark,
                        ReasonId = model.ReasonId,
                        LateInMins = model.RequestType == (short)RequestType.LateIn ? model.Minute : 0,
                        EarlyOutMins = model.RequestType == (short)RequestType.EarlyOut ? model.Minute : 0,
                        VerifierId = model.VerifierId,
                        ApproverId = model.ApproverId,
                        ModifiedBy = sessionService.Employee.Id
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria);
                    if (result.IsSuccessful)
                    {
                        lateInEarlyOutRequestService.SendMailEditRequest(model.RequestId);
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

        [HttpPost]
        public CustomJsonResult Delete(int id)
        {
            lateInEarlyOutRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = lateInEarlyOutRequestService.Delete(id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            var requests = lateInEarlyOutRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
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

        private void CheckIsTopManager(LateInEarlyOutRequestViewModel model)
        {
            Employee employee = employeeService.GetEmployeeById(model.EmployeeId);
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