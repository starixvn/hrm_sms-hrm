namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class ChangeShiftController : BaseController
    {
        private readonly IChangeShiftRequestService changeShiftService;
        private readonly IRequestService requestService;
        private readonly IShiftService shiftService;

        public ChangeShiftController(
            ISessionService sessionService,
            IChangeShiftRequestService changeShiftService,
            IRequestService requestService,
            IShiftService shiftService) : base(sessionService)
        {
            this.changeShiftService = changeShiftService;
            this.requestService = requestService;
            this.shiftService = shiftService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            ViewBag.Year = tempYear;
            var criteria = new GetRequestCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(tempYear, 1, 1),
                ToDate = new DateTime(tempYear, 12, 31),
                RequestType = (int)RequestFilter.ChangeShift,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = sessionService.Employee.Id,
                IsLocal = null
            };
            IList<RequestEntity> entityList = requestService.GetRequests(criteria);

            return View(entityList);
        }

        public ActionResult Create()
        {
            var today = DateTime.Today;
            VerifierAndApproverForRequest supervisor = GetVerifierApprover(sessionService.Employee.Id);
            var model = new ChangeShiftViewModel
            {
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                FromDate = today,
                ToDate = today,
                TotalRequestInMonth = changeShiftService.GetNumberOfRequest(sessionService.Employee.Id, today.FirstDayOfMonth(), today.LastDayOfMonth()),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.IsHost = true;
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public CustomJsonResult Create(CreateChangeShift model)
        {
            CommandResult commandResult = changeShiftService.Create(sessionService.Employee.Id, model);
            if (commandResult.IsSuccessful)
            {
                changeShiftService.SendMailAfterCreateRequest(commandResult.OutputId);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(int id)
        {
            ChangeShiftRequest request = changeShiftService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Employee.Id)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(sessionService.Employee.Id);
            var model = new ChangeShiftViewModel
            {
                RequestId = request.RequestId,
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                VerifierId = request.VerifiedBy.Id,
                ApproverId = request.ApprovedBy.Id,
                Remark = request.Remark,
                Details = changeShiftService.GetDetailById(id),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };

            if (request.TypeId == (byte)ChangeShiftType.MoveOffDay)
            {
                foreach (ChangeShiftDetail detail in model.Details)
                {
                    detail.DisableDates = changeShiftService.GetDisableDateForMoveOffDayRequest(model.EmployeeId, detail.OnDate, model.RequestId);
                }
            }

            ViewBag.IsHost = true;
            ViewBag.IsTopManager = request.Employee.IsTopManager;
            ViewBag.TypeId = request.TypeId;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public CustomJsonResult Edit(EditChangeShift model)
        {
            CommandResult commandResult = changeShiftService.Edit(sessionService.Employee.Id, model);
            if (commandResult.IsSuccessful)
            {
                changeShiftService.SendMailAfterEditRequest(model.RequestId);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult Delete(long id)
        {
            changeShiftService.SendMailAfterDeleteRequest(id);
            CommandResult commandResult = changeShiftService.Delete(id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Details(int id)
        {
            ChangeShiftRequest request = changeShiftService.GetById(id);
            if (request == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(sessionService.Employee.Id);
            var model = new ChangeShiftViewModel
            {
                Employee = request.Employee.DisplayName,
                EmployeeId = request.Employee.Id,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                VerifierId = request.VerifiedBy.Id,
                ApproverId = request.ApprovedBy.Id,
                Remark = request.Remark,
                Details = changeShiftService.GetDetailById(id),
                Status = request.Status,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.IsHost = false;
            ViewBag.TypeId = request.TypeId;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
        }

        public ActionResult GetChangeShiftDetail(GetWorkingCalendarForRequestParam param)
        {
            IEnumerable<DailySchedule> dailySchedules = changeShiftService.GetWorkingCalendarForRequest(param);
            IList<ChangeShiftDetail> details = dailySchedules.Select(x => new ChangeShiftDetail
            {
                OnDate = x.WorkDate,
                Remark = string.Empty,
                FromShiftId = GetFromShiftId(x),
                FromShift = GetFromShiftName(x)
            }).ToList();

            if (param.TypeId == (byte)ChangeShiftType.MoveOffDay)
            {
                foreach (ChangeShiftDetail detail in details)
                {
                    detail.DisableDates = changeShiftService.GetDisableDateForMoveOffDayRequest(param.EmployeeId, detail.OnDate, param.RequestId);
                }
            }

            ViewBag.IsHost = true;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            ViewBag.TypeId = param.TypeId;

            return PartialView("_ChangeShiftDetail", details);
        }

        public ActionResult ViewHistory(long id)
        {
            IEnumerable<ChangeShiftRequest> requests = changeShiftService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveRequest)]
        [HRMPermission(Policy.Host, FunctionKey.ProcessRequest)]
        [HRMPermission(Policy.Host, FunctionKey.VoidRequest)]
        public JsonResult ProcessRequest(ProcessRequestParam model)
        {
            CommandResult commandResult;
            switch (model.Command)
            {
                case RequestCommand.Accept:
                case RequestCommand.Approve:
                case RequestCommand.Process:
                    commandResult = changeShiftService.ApproveRequest(sessionService.Employee.Id, model);
                    break;

                case RequestCommand.Reset:
                    commandResult = changeShiftService.ResetRequest(sessionService.Employee.Id, model);
                    break;

                case RequestCommand.NotAccept:
                case RequestCommand.Reject:
                case RequestCommand.Void:
                    commandResult = changeShiftService.RejectRequest(sessionService.Employee.Id, model);
                    break;

                default:
                    commandResult = changeShiftService.VoidRequest(sessionService.Employee.Id, model);
                    break;
            }

            if (commandResult.IsSuccessful)
            {
                changeShiftService.SendMailAfterProcessRequest(model.RequestId, sessionService.Employee, model.Command);
            }

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        private static int GetFromShiftId(DailySchedule dailySchedule)
        {
            if (dailySchedule.IsOffDay)
            {
                return Constants.OffDayShiftId;
            }

            return dailySchedule.Shift != null ? dailySchedule.Shift.Id : 0;
        }

        private static string GetFromShiftName(DailySchedule dailySchedule)
        {
            if (dailySchedule.IsOffDay)
            {
                return Labels.Off;
            }

            return dailySchedule.Shift != null ? dailySchedule.Shift.Name : Labels.NA;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId, RequestEntity request = null)
        {
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(employeeId, RequestType.ChangeShift);

            if (request != null)
            {
                if (request.VerifiedBy != null && !supervisor.Verifiers.Any(t => t.OwnerId == request.VerifiedBy.Id))
                {
                    var verifier = new SupervisorNameModel
                    {
                        OwnerId = request.VerifiedBy.Id,
                        NickName = request.VerifiedBy.NickName,
                        FullName = request.VerifiedBy.FullName
                    };
                    supervisor.Verifiers.Add(verifier);
                }

                if (request.ApprovedBy != null && !supervisor.Approvers.Any(t => t.OwnerId == request.ApprovedBy.Id))
                {
                    var approver = new SupervisorNameModel
                    {
                        OwnerId = request.ApprovedBy.Id,
                        NickName = request.ApprovedBy.NickName,
                        FullName = request.ApprovedBy.FullName
                    };
                    supervisor.Approvers.Add(approver);
                }
            }

            return supervisor;
        }
    }
}
