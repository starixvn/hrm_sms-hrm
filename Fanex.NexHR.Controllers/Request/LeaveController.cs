namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class LeaveController : BaseController
    {
        private readonly ILeaveRequestService leaveRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly ICommonDataService commonDataService;

        public LeaveController(
            ISessionService sessionService,
            ILeaveRequestService leaveRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IEmployeeService employeeService,
            ICommonDataService commonDataService) : base(sessionService)
        {
            this.leaveRequestService = leaveRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.employeeService = employeeService;
            this.commonDataService = commonDataService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTime.Today.Year;
            var criteria = new GetLeaveRequestByDateCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(tempYear, 1, 1),
                ToDate = new DateTime(tempYear, 12, 31)
            };

            IList<LeaveRequest> leaves = leaveRequestService.GetByDate(criteria);

            ViewBag.Year = tempYear;
            ViewBag.EmployeeBalance = GetLeaveBalance(null, year);
            return View(leaves);
        }

        public ActionResult Create()
        {
            VerifierAndApproverForRequest supervisor = GetVerifierApprover(sessionService.Employee.Id);
            var model = new LeaveRequestViewModel
            {
                LeaveTypeId = Constants.GeneralLeave,
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypeList = commonDataService.GetLeaveType(),
                EmployeeLeaveBalance = GetLeaveBalance(null, null),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(string request, string requestDetails)
        {
            InsertLeaveRequestCriteria criteria = JsonConverter.FromJson<InsertLeaveRequestCriteria>(request);
            List<LeaveRequestDetail> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveRequestDetail>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.CreatedBy = sessionService.Employee.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);
            if (result.IsSuccessful)
            {
                leaveRequestService.SendMailCreateRequest(criteria);
            }

            return Json(result);
        }

        public ActionResult Update(long id)
        {
            LeaveRequest request = leaveRequestService.GetById(id);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            if (model == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, model.EmployeeId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            model.UnavailDates = GetUnavailDate(model);
            model.LeaveTypeList = commonDataService.GetLeaveType();
            model.EmployeeLeaveBalance = GetLeaveBalance(id, null);

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(sessionService.Employee.Id);
            model.Verifiers = supervisor.Verifiers;
            model.Approvers = supervisor.Approvers;

            ViewBag.IsView = false;
            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string request, string requestDetails)
        {
            UpdateLeaveRequestCriteria criteria = JsonConverter.FromJson<UpdateLeaveRequestCriteria>(request);
            List<LeaveRequestDetail> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveRequestDetail>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.ModifiedBy = sessionService.Employee.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                leaveRequestService.SendMailEditRequest(criteria);
            }

            return Json(result);
        }

        public ActionResult Detail(long id)
        {
            LeaveRequest request = leaveRequestService.GetById(id);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            model.LeaveTypeList = commonDataService.GetLeaveType();

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(sessionService.Employee.Id);
            model.Verifiers = supervisor.Verifiers;
            model.Approvers = supervisor.Approvers;
            ViewBag.IsView = true;
            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        public CustomJsonResult Delete(long id)
        {
            leaveRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = leaveRequestService.Delete(id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            IList<LeaveRequest> requests = leaveRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public ActionResult GetLeaveBalance(int employeeId, long? requestId, DateTime? toDate)
        {
            var criteria = new GetLeaveBalanceCriteria
            {
                EmpId = employeeId,
                RequestId = requestId == -1 ? null : requestId,
                EffectedDay = toDate ?? DateTime.Now.Date
            };
            EmployeeLeaveBalance employeeLeaveBalance = leaveRequestService.GetLeaveBalance(criteria);
            return Json(employeeLeaveBalance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveRequestDetail(int employeeId, DateTime fromDate, DateTime toDate, long requestId = -1)
        {
            IEnumerable<Models.ScheduleArrange.DailySchedule> calendars = leaveRequestService.GetEmployeeCalendar(employeeId, fromDate, toDate);
            IList<LeaveRequestDetail> leaveRequestDetail = new List<LeaveRequestDetail>();
            if (requestId != -1)
            {
                leaveRequestDetail = leaveRequestService.GetLeaveRequestDetailById(requestId);
            }

            leaveRequestDetail = ConvertToLeaveRequestDetail(calendars, leaveRequestDetail, fromDate, toDate);
            IList<LeaveType> leaveTypes = commonDataService.GetLeaveType();
            var model = new LeaveDetailViewModel
            {
                LeaveRequestDetail = leaveRequestDetail,
                LeaveTypeList = leaveTypes.Where(x => x.Id != (int)LeaveTypes.GeneralLeave
                                        && x.Id != (int)LeaveTypes.KeepInViewByHour
                                        && x.Id != (int)LeaveTypes.BusinessDuringWorking).ToList()
            };

            return View(model);
        }

        private List<LeaveRequestDetail> ConvertToLeaveRequestDetail(
            IEnumerable<Models.ScheduleArrange.DailySchedule> calendars,
            IList<LeaveRequestDetail> leaveDetails,
            DateTime fromDate,
            DateTime toDate)
        {
            var details = new List<LeaveRequestDetail>();
            if (fromDate <= toDate)
            {
                var numberDays = (toDate - fromDate).Days + 1;
                DateTime tempDate;

                for (int i = 0; i < numberDays; i++)
                {
                    tempDate = fromDate.AddDays(i);
                    IEnumerable<LeaveRequestDetail> tempLeaveDetailModels = leaveDetails.Where(t => (t.OnDate == tempDate));
                    Models.ScheduleArrange.DailySchedule calendar = calendars.FirstOrDefault(t => t.WorkDate.Date == tempDate);

                    if (calendar != null)
                    {
                        AddLeaveRequestExistCalendar(details, tempLeaveDetailModels, calendar);
                    }
                    else
                    {
                        AddLeaveRequestNotExistCalendar(details, tempLeaveDetailModels, tempDate);
                    }
                }
            }

            return details.OrderBy(t => t.OnDate).ThenBy(t => t.FromTime).ToList();
        }

        private void AddLeaveRequestExistCalendar(
           List<LeaveRequestDetail> leaveRequestDetailOutput,
           IEnumerable<LeaveRequestDetail> leaveDetails,
           Models.ScheduleArrange.DailySchedule calendar)
        {
            string tempScheduleStatus = string.Empty;
            LeaveRequestDetail tempLeaveDetailModel = null;
            var isOffOrHoliday = calendar.IsOffDay || calendar.IsHoliday;

            if (!calendar.IsLeaveDay)
            {
                tempScheduleStatus = Labels.NA;
            }

            if (calendar.IsOffDay)
            {
                tempScheduleStatus = Labels.OffDay;
            }

            if (calendar.IsHoliday)
            {
                tempScheduleStatus = Labels.Holiday;
            }

            if (calendar.Shift != null)
            {
                tempScheduleStatus = calendar.Shift.Name;
            }

            if (leaveDetails != null && leaveDetails.Any())
            {
                foreach (LeaveRequestDetail leaveDetail in leaveDetails)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.IsOffOrHoliday = isOffOrHoliday;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.OnDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.OnDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveRequestDetail
                {
                    OnDate = calendar.WorkDate,
                    ScheduleStatus = tempScheduleStatus,
                    IsOffOrHoliday = isOffOrHoliday,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = (byte)WorkType.Leave
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        private void AddLeaveRequestNotExistCalendar(
            List<LeaveRequestDetail> leaveRequestDetailOutput,
            IEnumerable<LeaveRequestDetail> tempLeaveDetailModels,
            DateTime workDate)
        {
            string tempScheduleStatus = Labels.NA;
            LeaveRequestDetail tempLeaveDetailModel = null;

            if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
            {
                foreach (LeaveRequestDetail leaveDetail in tempLeaveDetailModels)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.OnDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.OnDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveRequestDetail
                {
                    OnDate = workDate,
                    ScheduleStatus = Labels.NA,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = (byte)WorkType.Leave
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId, LeaveRequest request = null)
        {
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(employeeId, RequestType.Leave);

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

        private LeaveRequestViewModel ConvertRequestToModel(LeaveRequest request)
        {
            LeaveRequestViewModel model = null;
            if (request != null)
            {
                var fromTime = leaveRequestService.GetFromTimeHalfDay(request.LeaveDetail);
                var toTime = leaveRequestService.GetToTimeHalfDay(request.LeaveDetail);
                model = new LeaveRequestViewModel
                {
                    RequestId = request.RequestId,
                    LeaveTypeId = request.LeaveType.Id,
                    Employee = request.Employee.DisplayName,
                    EmployeeId = request.Employee.Id,
                    FromDate = request.FromDate,
                    ToDate = (request.LeaveType.Id != (int)LeaveTypes.GeneralLeave
                            && request.IsHalfDay
                            && request.ToDate.Date != toTime) ? request.ToDate.AddDays(1) : request.ToDate,
                    TotalDays = request.TotalDays,
                    HalfDayOff = request.IsHalfDay,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    RefNo = request.RefNO,
                    Remark = request.Remark,
                    FromTime = fromTime.ToString("HH:mm"),
                    ToTime = toTime.ToString("HH:mm"),
                    FromTimeHour = fromTime.Hour,
                    FromTimeMinute = fromTime.Minute,
                    ToTimeHour = toTime.Hour,
                    ToTimeMinute = toTime.Minute,
                    IsFromTimeAfterDay = request.FromDate.Date != fromTime.Date,
                    IsToTimeAfterDay = request.ToDate.Date != toTime.Date,
                    Status = request.Status
                };
            }

            var today = DateTime.Today;
            var minDate = today.FirstDayOfMonth().AddMonths(-5);
            var maxDate = today.FirstDayOfMonth().AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest).AddDays(-1);
            if (model != null)
            {
                if (model.LeaveTypeId == Constants.MaternityLeave)
                {
                    //// rule: user can make maternity leave for 6 months.
                    const byte ExtraMonthsCanRequest = 2;
                    maxDate = maxDate.AddMonths(ExtraMonthsCanRequest);
                }

                model.MinDate = minDate > model.FromDate ? model.FromDate : minDate;
                model.MaxDate = maxDate;
            }

            return model;
        }

        private EmployeeLeaveBalance GetLeaveBalance(long? requestId, int? year)
        {
            var criteria = new GetLeaveBalanceCriteria
            {
                EmpId = sessionService.Employee.Id,
                RequestId = requestId,
                EffectedDay = year.HasValue ? new DateTime(year.Value, 1, 1) : DateTime.Now.Date
            };
            return leaveRequestService.GetLeaveBalance(criteria);
        }

        private IList<DateTime> GetUnavailDate(LeaveRequestViewModel model)
        {
            var criteria = new GetUnavailableRequestDateCriteria
            {
                EmpId = model.EmployeeId,
                FromDate = model.MinDate,
                ToDate = model.MaxDate
            };

            IList<DateTime> unavailDates = model.LeaveTypeId == Constants.GeneralLeave
                ? new List<DateTime> { DateTime.Now.AddYears(-3) }
                : employeeService.GetUnavailableDates(criteria);
            return unavailDates;
        }
    }
}