namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class LateInEarlyOutController : BaseController
    {
        private const int DefaultMinutes = 60;
        private readonly ILateInEarlyOutRequestService lateInEarlyOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;

        public LateInEarlyOutController(
            ISessionService sessionService,
            ILateInEarlyOutRequestService lateInEarlyOutRequestService,
            IRequestService requestService,
            IBaseService baseService) : base(sessionService)
        {
            this.lateInEarlyOutRequestService = lateInEarlyOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
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

        public ActionResult Create()
        {
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.LateIn);
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                OnDate = DateTime.Today,
                Minute = DefaultMinutes,
                ReasonId = (byte)LateInEarlyOutReasons.Other,
                RequestType = (byte)RequestType.LateIn,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Create(LateInEarlyOutRequestViewModel model)
        {
            var criteria = new InsertLateInEarlyOutRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.OnDate,
                RequestType = model.RequestType,
                Remark = model.Remark,
                LateInMins = model.RequestType == (byte)RequestType.LateIn ? model.Minute : 0,
                EarlyOutMins = model.RequestType == (byte)RequestType.EarlyOut ? model.Minute : 0,
                ReasonId = model.ReasonId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                CreatedBy = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);

            if (commandResult.IsSuccessful)
            {
                lateInEarlyOutRequestService.SendMailCreateRequest(commandResult.OutputId);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(int id)
        {
            LateInEarlyOutRequest request = lateInEarlyOutRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Employee.Id)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.LateIn);
            var minutes = request.RequestTypeId == (byte)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins;
            IEnumerable<DateTime> disabledDates = lateInEarlyOutRequestService.GetDisabledDate(new GetDisabledDateParam
            {
                EmployeeId = request.Employee.Id,
                FromDate = request.OnDate,
                ToDate = DateTime.Today,
                RequestId = request.RequestId
            });
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                RequestId = request.RequestId,
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                OnDate = request.OnDate,
                Minute = minutes,
                RequestType = request.RequestTypeId,
                Remark = request.Remark,
                ReasonId = request.ReasonId,
                ApproverId = request.ApprovedBy?.Id,
                VerifierId = request.VerifiedBy?.Id,
                DisabledDates = disabledDates,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };

            ViewBag.IsTopManager = request.Employee.IsTopManager;
            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Edit(LateInEarlyOutRequestViewModel model)
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

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                lateInEarlyOutRequestService.SendMailEditRequest(model.RequestId);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        public CustomJsonResult GetActualLateInEarlyOut(int employeeId, DateTime workDate)
        {
            var criteria = new GetActualLateInEarlyOutCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate
            };

            EmployeeLateInEarlyOutModel lateInEarlyOut = lateInEarlyOutRequestService.GetActualLateInEarlyOut(criteria);
            lateInEarlyOut = lateInEarlyOut ?? new EmployeeLateInEarlyOutModel();

            return new CustomJsonResult { Data = new { lateInEarlyOut.LateIn, lateInEarlyOut.EarlyOut }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = lateInEarlyOutRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}