namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.LeaveType;
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
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly ILeaveTypeService leaveTypeService;

        public LeaveController(
            ISessionService sessionService,
            ILeaveRequestService leaveRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IEmployeeService employeeService,
            IWorkingCalendarService workingCalendarService,
            ILeaveTypeService leaveTypeService) : base(sessionService)
        {
            this.leaveRequestService = leaveRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.employeeService = employeeService;
            this.workingCalendarService = workingCalendarService;
            this.leaveTypeService = leaveTypeService;
        }

        public ActionResult Create()
        {
            var model = new LeaveRequestViewModel
            {
                LeaveTypeId = Constants.GeneralLeave,
                Employee = sessionService.Employee.DisplayName,
                EmployeeId = sessionService.Employee.Id,
                FromDate = DateTime.Now.Date,
                ToDate = DateTime.Now.Date,
                TotalDays = 1,
                LeaveTypeList = GetAllLeaveType(),
                TotalRequest = GetNumberRequestInMonth(),
                EmployeeLeaveBalance = GetLeaveBalance(null, null),
                VerifierAndApprover = GetVerifierApprover(sessionService.Employee.Id)
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
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveDetailModel>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.CreatedBy = sessionService.Employee.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);
            if (result.IsSuccessful)
            {
                leaveRequestService.SendMailForInsertSuccessfully(criteria);
            }

            return Json(result);
        }

        public ActionResult Update(long requestId)
        {
            var criteria = new GetRequestByIdCriteria
            {
                RequestId = requestId
            };
            RequestEntity request = requestService.GetRequestById(criteria);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            if (model == null || !Utilities.CheckUserCanEditRequest(sessionService.Employee.Id, model.EmployeeId))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            model.UnavailDates = GetUnavailDate(model);
            model.LeaveTypeList = GetAllLeaveType();
            model.EmployeeLeaveBalance = GetLeaveBalance(requestId, null);
            model.VerifierAndApprover = GetVerifierApprover(sessionService.Employee.Id);

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
            List<LeaveDetailModel> leaveRequestDetail = JsonConverter.FromJsonArray<LeaveDetailModel>(requestDetails);
            criteria.RequestDetail = leaveRequestDetail;
            criteria.ModifiedBy = sessionService.Employee.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                leaveRequestService.SendMailForUpdateSuccessfully(criteria);
            }

            return Json(result);
        }

        public ActionResult Detail(int requestId)
        {
            var criteria = new GetRequestByIdCriteria
            {
                RequestId = requestId
            };
            RequestEntity request = requestService.GetRequestById(criteria);
            LeaveRequestViewModel model = ConvertRequestToModel(request);
            model.LeaveTypeList = GetAllLeaveType();
            model.VerifierAndApprover = GetVerifierApprover(request.Requester.Id, request);
            ViewBag.IsView = true;
            ViewBag.HasOnBehalfOthersPermission = sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || sessionService.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

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
            var criteria = new GetRequestCriteria
            {
                SupervisorId = sessionService.Employee.Id,
                EmpId = sessionService.Employee.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                RequestType = (int)RequestFilter.Leave,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                IsLocal = null
            };

            IList<RequestEntity> entityList = requestService.GetRequests(criteria);

            ViewBag.Year = year;
            ViewBag.EmployeeBalance = GetLeaveBalance(null, year);
            return View(entityList);
        }

        public ActionResult GetLeaveBalance(int employeeId, long? requestId, DateTime? toDate)
        {
            var criteria = new GetLeaveBalanceCriteria
            {
                EmpId = employeeId,
                RequestId = requestId == -1 ? null : requestId,
                EffectedDay = toDate.HasValue ? toDate.Value : DateTime.Now.Date
            };
            EmployeeLeaveBalance employeeLeaveBalance = leaveRequestService.GetLeaveBalance(criteria);
            return Json(employeeLeaveBalance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveRequestDetail(int employeeId, DateTime fromDate, DateTime toDate, long requestId = -1)
        {
            var criteria = new GetWorkingCalendarByDateRangeCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupIds = string.Empty,
                EmpId = employeeId
            };
            IList<WorkingCalendarModel> calendars = workingCalendarService.GetWorkingCalendarByDateRange(criteria);
            IList<LeaveDetailModel> leaveRequestDetail = new List<LeaveDetailModel>();
            if (requestId != -1)
            {
                var leaveDetailCriteria = new GetLeaveRequestDetailByIdCriteria
                {
                    RequestId = requestId
                };

                leaveRequestDetail = leaveRequestService.GetLeaveRequestDetailById(leaveDetailCriteria);
            }

            leaveRequestDetail = ConvertToLeaveRequestDetail(calendars, leaveRequestDetail, fromDate, toDate);
            var model = new LeaveDetailViewModel
            {
                LeaveRequestDetail = leaveRequestDetail,
                LeaveTypeList = GetAllLeaveType()
            };

            return View(model);
        }

        private List<LeaveDetailModel> ConvertToLeaveRequestDetail(
            IList<WorkingCalendarModel> workingCalendars,
            IList<LeaveDetailModel> leaveRequestDetailInput,
            DateTime fromDate,
            DateTime toDate)
        {
            var leaveRequestDetailOutput = new List<LeaveDetailModel>();
            if (fromDate <= toDate)
            {
                var numberDays = (toDate - fromDate).Days + 1;
                DateTime tempDate;

                for (int i = 0; i < numberDays; i++)
                {
                    tempDate = fromDate.AddDays(i);
                    IEnumerable<LeaveDetailModel> tempLeaveDetailModels = leaveRequestDetailInput.Where(t => (t.LeaveDate == tempDate));
                    IEnumerable<WorkingCalendarModel> tempWokingCalendars = workingCalendars.Where(t => t.WorkDate.Date == tempDate);

                    if (tempWokingCalendars != null && tempWokingCalendars.Any())
                    {
                        WorkingCalendarModel tempWokingCalendar = tempWokingCalendars.FirstOrDefault();
                        AddLeaveRequestExistCalendar(leaveRequestDetailOutput, tempLeaveDetailModels, tempWokingCalendar);
                    }
                    else
                    {
                        AddLeaveRequestNotExistCalendar(leaveRequestDetailOutput, tempLeaveDetailModels, tempDate);
                    }
                }
            }

            return leaveRequestDetailOutput.OrderBy(t => t.LeaveDate).ThenBy(t => t.FromTime).ToList();
        }

        private void AddLeaveRequestExistCalendar(
           List<LeaveDetailModel> leaveRequestDetailOutput,
           IEnumerable<LeaveDetailModel> tempLeaveDetailModels,
           WorkingCalendarModel tempWokingCalendar)
        {
            string tempScheduleStatus = string.Empty;
            LeaveDetailModel tempLeaveDetailModel = null;
            var isOffOrHoliday = tempWokingCalendar.IsOffDay || tempWokingCalendar.IsHoliday;

            if (!tempWokingCalendar.IsLeaveDay)
            {
                tempScheduleStatus = Labels.NA;
            }

            if (tempWokingCalendar.IsOffDay)
            {
                tempScheduleStatus = Labels.OffDay;
            }

            if (tempWokingCalendar.IsHoliday)
            {
                tempScheduleStatus = Labels.Holiday;
            }

            if (tempWokingCalendar.Shift != null)
            {
                tempScheduleStatus = tempWokingCalendar.Shift.Name;
            }

            if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
            {
                foreach (LeaveDetailModel leaveDetail in tempLeaveDetailModels)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.IsOffOrHoliday = isOffOrHoliday;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.LeaveDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.LeaveDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveDetailModel
                {
                    LeaveDate = tempWokingCalendar.WorkDate,
                    ScheduleStatus = tempScheduleStatus,
                    IsOffOrHoliday = isOffOrHoliday,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = 2
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        private void AddLeaveRequestNotExistCalendar(
            List<LeaveDetailModel> leaveRequestDetailOutput,
            IEnumerable<LeaveDetailModel> tempLeaveDetailModels,
            DateTime workDate)
        {
            string tempScheduleStatus = Labels.NA;
            LeaveDetailModel tempLeaveDetailModel = null;

            if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
            {
                foreach (LeaveDetailModel leaveDetail in tempLeaveDetailModels)
                {
                    tempLeaveDetailModel = leaveDetail;
                    tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                    tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.LeaveDate.Date;
                    tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.LeaveDate.Date;
                    leaveRequestDetailOutput.Add(tempLeaveDetailModel);
                }
            }
            else
            {
                tempLeaveDetailModel = new LeaveDetailModel
                {
                    LeaveDate = workDate,
                    ScheduleStatus = Labels.NA,
                    LeaveTypeId = Constants.AnnualLeave,
                    WorkType = 2
                };
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId, RequestEntity request = null)
        {
            var criteria = new GetVerifierAndApproverCriteria
            {
                EmpId = employeeId,
                RequestType = (byte)RequestType.Leave
            };
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(criteria);

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

        private IList<LeaveType> GetAllLeaveType()
        {
            var criteria = new GetLeaveTypeCriteria
            {
                ExcludeNotShown = true
            };
            IList<LeaveType> leaveTypes = leaveTypeService.GetAll(criteria);
            return leaveTypes;
        }

        private LeaveRequestViewModel ConvertRequestToModel(RequestEntity request)
        {
            LeaveRequestViewModel model = null;
            if (request != null)
            {
                model = new LeaveRequestViewModel
                {
                    RequestId = request.Id,
                    LeaveTypeId = request.LeaveType.Id,
                    Employee = request.Requester.DisplayName,
                    EmployeeId = request.Requester.Id,
                    FromDate = request.FromDate,
                    ToDate = (request.LeaveType.Id != (int)LeaveTypes.GeneralLeave
                            && request.IsHalfDay
                            && request.ToDate.Date != request.ToTime.Date) ? request.ToDate.AddDays(1) : request.ToDate,
                    TotalDays = request.TotalDays,
                    HalfDayOff = request.IsHalfDay,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    RefNo = request.RefNO,
                    Remark = request.Remark,
                    FromTime = request.FromTime.ToString("HH:mm"),
                    ToTime = request.ToTime.ToString("HH:mm"),
                    IsFromTimeAfterDay = request.FromDate.Date != request.FromTime.Date,
                    IsToTimeAfterDay = request.ToDate.Date != request.ToTime.Date,
                    Status = request.Status
                };
            }

            var minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-5);
            var maxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest).AddDays(-1);
            if (model != null)
            {
                if (model.LeaveTypeId == Constants.MaternityLeave)
                {
                    //// rule: user can make leave request (maternity leave) for 6 months.
                    maxDate = maxDate.AddMonths(2);
                }

                model.MinDate = minDate > model.FromDate ? model.FromDate : minDate;
                model.MaxDate = maxDate;
            }

            return model;
        }

        private EmployeeLeaveBalance GetLeaveBalance(long? requestId, int? year)
        {
            var getLeaveBalanceCriteria = new GetLeaveBalanceCriteria
            {
                EmpId = sessionService.Employee.Id,
                RequestId = requestId,
                EffectedDay = year.HasValue ? new DateTime(year.Value, 1, 1) : DateTime.Now.Date
            };

            EmployeeLeaveBalance employeeLeaveBalance = leaveRequestService.GetLeaveBalance(getLeaveBalanceCriteria);
            return employeeLeaveBalance;
        }

        private decimal GetNumberRequestInMonth()
        {
            var currentDate = DateTime.Now.Date;
            var fromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var criteria = new GetNumberRequestInMonthCriteria
            {
                EmployeeId = sessionService.Employee.Id,
                RequestType = (int)RequestType.Leave,
                FromDate = fromDate,
                ToDate = toDate
            };
            decimal numberRequests = requestService.GetNumberRequestInMonth(criteria);

            return numberRequests;
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