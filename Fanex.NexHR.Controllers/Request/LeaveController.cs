namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class LeaveController : BaseController
    {
        private readonly ILeaveRequestService leaveRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly ICommonDataService commonDataService;

        public LeaveController(
            ISessionService sessionService,
            ILeaveRequestService leaveRequestService,
            IRequestService requestService,
            IBaseService baseService,
            ICommonDataService commonDataService) : base(sessionService)
        {
            this.leaveRequestService = leaveRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
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
            foreach (var request in leaves)
            {
                request.ContentFile = requestService.GetInfoAttachFile(request.RequestId);
            }
            ViewBag.Year = tempYear;
            ViewBag.EmployeeBalance = GetLeaveBalance(null, year);
            return View(leaves);
        }

        public ActionResult Create()
        {
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(sessionService.Employee.Id, RequestType.Leave);
            var model = new LeaveRequestViewModel
            {
                LeaveTypeId = (ushort)LeaveTypes.GeneralLeave,
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypeList = commonDataService.GetLeaveType(),
                EmployeeLeaveBalance = GetLeaveBalance(null, null),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                MinDate = DateTime.Today.FirstDayOfMonth().AddMonths(-5)
            };
            ViewBag.IsTopManager = sessionService.Employee.IsTopManager;
            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(string request, string requestDetails, IList<HttpPostedFileBase> files)
        {
            var criteria = JsonConverter.FromJson<InsertLeaveRequestCriteria>(request);
            var leaveRequestDetail = JsonConverter.FromJsonArray<LeaveRequestDetail>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.CreatedBy = sessionService.Employee.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                leaveRequestService.SendMailCreateRequest(criteria);
                requestService.UploadFile(files, criteria.RequestId);
            }

            return Json(result);
        }

        public ActionResult Update(long id)
        {
            LeaveRequest request = leaveRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            LeaveRequestViewModel model = ConvertRequestToModel(request);
            model.DisabledDates = leaveRequestService.GetDisabledDate(new GetDisabledDateParam
            {
                EmployeeId = model.EmployeeId,
                FromDate = model.MinDate,
                ToDate = model.MaxDate,
                RequestId = model.RequestId
            });
            model.LeaveTypeList = commonDataService.GetLeaveType();
            model.EmployeeLeaveBalance = GetLeaveBalance(id, null);

            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(model.EmployeeId, RequestType.Leave);
            model.Verifiers = supervisor.Verifiers;
            model.Approvers = supervisor.Approvers;
            model.Files = requestService.GetAttachFile(id);

            ViewBag.IsView = false;
            ViewBag.IsTopManager = request.Requester.IsTopManager;
            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string request, string requestDetails, IList<HttpPostedFileBase> files, string filesRemove)
        {
            var criteria = JsonConverter.FromJson<UpdateLeaveRequestCriteria>(request);
            var leaveRequestDetail = JsonConverter.FromJsonArray<LeaveRequestDetail>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.ModifiedBy = sessionService.Employee.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                leaveRequestService.SendMailEditRequest(criteria);
                requestService.UploadFile(files, criteria.RequestId);
                if (!string.IsNullOrWhiteSpace(filesRemove))
                {
                    requestService.DeleteFile(JsonConverter.FromJsonArray<string>(filesRemove), criteria.RequestId);
                }
            }

            return Json(result);
        }

        public ActionResult Detail(long id)
        {
            LeaveRequest request = leaveRequestService.GetById(id);
            if (request == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            LeaveRequestViewModel model = ConvertRequestToModel(request);
            model.LeaveTypeList = commonDataService.GetLeaveType();
            model.Files = requestService.GetAttachFile(id);

            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(request.Requester.EmpId, RequestType.Leave);
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

            if (commandResult.IsSuccessful)
            {
                requestService.DeleteFolderAttachFile(id);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            IEnumerable<LeaveRequest> requests = leaveRequestService.GetHistoryById(id);
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

        public CustomJsonResult GetDisableDate(int employeeId, DateTime date, long? requestId = null)
        {
            var fromDate = new DateTime(date.AddMonths(-5).Year, date.AddMonths(-5).Month, 1);
            var tempDate = date.AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest);
            var toDate = new DateTime(tempDate.Year, tempDate.Month, DateTime.DaysInMonth(tempDate.Year, tempDate.Month));

            var param = new GetDisabledDateParam
            {
                RequestId = requestId,
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            IEnumerable<DateTime> dates = leaveRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = dates.Select(t => t.Date.ToString("yyyy/MM/dd")), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetTotalLeaveDay(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> dates = leaveRequestService.GetDisabledDate(param);
            var totalDays = (param.ToDate - param.FromDate).Days + 1 - dates.Count();

            return new CustomJsonResult { Data = totalDays, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
                    LeaveTypeId = (ushort)LeaveTypes.AnnualLeave,
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
                    LeaveTypeId = (ushort)LeaveTypes.AnnualLeave,
                    WorkType = (byte)WorkType.Leave
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        private LeaveRequestViewModel ConvertRequestToModel(LeaveRequest request)
        {
            var model = new LeaveRequestViewModel
            {
                RequestId = request.RequestId,
                LeaveTypeId = request.LeaveType.Id,
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TotalDays = request.TotalDays,
                HalfDayOff = request.IsHalfDay,
                VerifierId = request.Verifier != null ? request.Verifier.EmpId : 0,
                ApproverId = request.Approver != null ? request.Approver.EmpId : 0,
                RefNo = request.RefNO,
                Remark = request.Remark,
                Status = request.Status
            };

            if (request.IsHalfDay)
            {
                GetTimeForHalfDayRequest(request, model);
            }

            var today = DateTime.Today;
            var minDate = today.FirstDayOfMonth().AddMonths(-5);
            var maxDate = today.FirstDayOfMonth().AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest).AddDays(-1);
            if (model.LeaveTypeId == (ushort)LeaveTypes.MaternityLeave)
            {
                //// rule: user can make maternity leave for 6 months.
                const byte ExtraMonthsCanRequest = 2;
                maxDate = maxDate.AddMonths(ExtraMonthsCanRequest);
            }

            model.MinDate = minDate > model.FromDate ? model.FromDate : minDate;
            model.MaxDate = maxDate;

            return model;
        }

        private void GetTimeForHalfDayRequest(LeaveRequest request, LeaveRequestViewModel model)
        {
            var fromTime = leaveRequestService.GetFromTimeHalfDay(request.LeaveDetail);
            var toTime = leaveRequestService.GetToTimeHalfDay(request.LeaveDetail);

            model.FromTime = fromTime.ToString("HH:mm");
            model.ToTime = toTime.ToString("HH:mm");
            model.FromTimeHour = fromTime.Hour;
            model.FromTimeMinute = fromTime.Minute;
            model.ToTimeHour = toTime.Hour;
            model.ToTimeMinute = toTime.Minute;
            model.IsFromTimeAfterDay = request.FromDate.Date != fromTime.Date;
            model.IsToTimeAfterDay = fromTime.Date != toTime.Date || request.ToDate.Date != toTime.Date;
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
    }
}