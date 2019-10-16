namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request.CheckInOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class CheckInOutController : BaseController
    {
        private readonly ICheckInOutRequestService checkInOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IShiftService shiftService;

        public CheckInOutController(
            ISessionService sessionService,
            ICheckInOutRequestService checkInOutRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IShiftService shiftService) : base(sessionService)
        {
            this.checkInOutRequestService = checkInOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.shiftService = shiftService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            ViewBag.Year = tempYear;
            var criteria = new GetCheckInOutRequestByDateCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(tempYear, 1, 1),
                ToDate = new DateTime(tempYear, 12, 31)
            };

            IList<CheckInOutRequest> requests = checkInOutRequestService.GetByDate(criteria);
            return View(requests);
        }

        public ActionResult Create()
        {
            var today = DateTime.Today;
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.CheckIn);
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                ForDate = today,
                AttendantDate = today,
                Hour = DateTime.Now.Hour,
                Minute = DateTime.Now.Minute,
                RequestTypeId = (byte)RequestType.CheckIn,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };

            Shift shift = shiftService.GetByDate(sessionService.Employee.Id, today);
            CheckShiftFromToFormat(viewModel, shift);
            viewModel.Shift = shift == null ? string.Empty : shift.Name;
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Create(CheckInOutRequestViewModel model)
        {
            var criteria = new InsertCheckInOutRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.AttendantDate,
                RequestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                RequestType = model.RequestTypeId,
                ReasonId = model.ReasonId,
                RefNo = model.RefNO,
                Remark = model.Remark,
                CreatedBy = sessionService.Employee.Id,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);

            if (commandResult.IsSuccessful)
            {
                checkInOutRequestService.SendMailCreateRequest(commandResult.OutputId);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(long id)
        {
            CheckInOutRequest request = checkInOutRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Employee.Id)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            else
            {
                VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.CheckIn);
                IEnumerable<DateTime> disabledDates = checkInOutRequestService.GetDisabledDate(new GetDisabledDateParam
                {
                    EmployeeId = request.Employee.Id,
                    FromDate = request.OnDate,
                    ToDate = DateTime.Today,
                    RequestId = request.RequestId
                });
                Shift shift = shiftService.GetByDate(sessionService.Employee.Id, request.OnDate);
                var viewModel = new CheckInOutRequestViewModel
                {
                    RequestId = request.RequestId,
                    RefNO = request.RefNO,
                    RequestTypeId = request.RequestTypeId,
                    Employee = sessionService.Employee.DisplayName,
                    EmployeeId = sessionService.Employee.Id,
                    AttendantDate = request.OnDate,
                    ForDate = request.RequestTime.Date,
                    Hour = request.RequestTime.Hour,
                    Minute = request.RequestTime.Minute,
                    ReasonId = request.ReasonId,
                    Shift = shift == null ? string.Empty : shift.Name,
                    ApproverId = request.ApprovedBy?.Id,
                    VerifierId = request.VerifiedBy?.Id,
                    Remark = request.Remark,
                    Verifiers = supervisor.Verifiers,
                    Approvers = supervisor.Approvers,
                    DisabledDates = disabledDates
                };
                ViewBag.IsTopManager = request.Employee.IsTopManager;
                return View(viewModel);
            }
        }

        [HttpPost]
        public CustomJsonResult Edit(CheckInOutRequestViewModel model)
        {
            var criteria = new UpdateCheckInOutRequestCriteria
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.AttendantDate,
                RequestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                RequestType = model.RequestTypeId,
                ReasonId = model.ReasonId,
                RefNo = model.RefNO,
                Remark = model.Remark,
                ModifiedBy = sessionService.Employee.Id,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                Shift = model.Shift
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                checkInOutRequestService.SendMailEditRequest(model.RequestId);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public CustomJsonResult Delete(int id)
        {
            checkInOutRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = checkInOutRequestService.Delete(id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            IList<CheckInOutRequest> requests = checkInOutRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = checkInOutRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private static void CheckShiftFromToFormat(CheckInOutRequestViewModel viewModel, Shift shift)
        {
            if (shift != null)
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
        }
    }
}