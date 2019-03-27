namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;

    public class ChangeShiftController : BaseController
    {
        private readonly IChangeShiftRequestService changeShiftRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IShiftService shiftService;
        private readonly IEmployeeService employeeService;
        private readonly IWorkingCalendarService workingCalendarService;

        public ChangeShiftController(
            IChangeShiftRequestService changeShiftRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IEmployeeService employeeService,
            IShiftService shiftService,
            IWorkingCalendarService workingCalendarService)
        {
            this.changeShiftRequestService = changeShiftRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.employeeService = employeeService;
            this.shiftService = shiftService;
            this.workingCalendarService = workingCalendarService;
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
                RequestType = (int)RequestFilter.ChangeShift,
                IsQueryByCreatedDate = false,
                RequestViewType = RequestViewType.CompanySpace,
                SupervisorId = CurrentUser.Id,
                IsLocal = null
            };

            IList<RequestEntity> entityList = requestService.GetRequests(criteria);

            return View(entityList);
        }

        public ActionResult Create()
        {
            var currentDate = DateTime.Now.Date;
            List<ChangeShiftDetailViewModel> shiftDetails = GetChangeShiftDetail(CurrentUser.Id, currentDate, currentDate);

            var model = new ChangeShiftViewModel
            {
                Employee = CurrentUser.NickName,
                EmployeeId = CurrentUser.Id,
                FromDate = currentDate,
                ToDate = currentDate,
                ShiftDetails = shiftDetails,
                TotalRequestInMonth = GetTotalRequestInMonth(CurrentUser.Id),
                VerifierAndApprover = GetVerifierApprover(CurrentUser.Id)
            };

            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create(string shiftJson, string shiftDetailJson)
        {
            var criteria = JsonConverter.FromJson<InsertChangeShiftRequestCriteria>(shiftJson);
            criteria.RequestDetail = JsonConverter.FromJsonArray<ChangeShiftDetailModel>(shiftDetailJson);
            criteria.CreatedBy = CurrentUser.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);
            if (result.IsSuccessful)
            {
                changeShiftRequestService.SendMailForInsertSuccessfully(criteria);
            }

            return Json(result);
        }

        public ActionResult Update(int id)
        {
            var getRequestByIdcriteria = new GetRequestByIdCriteria
            {
                RequestId = id
            };
            RequestEntity request = requestService.GetRequestById(getRequestByIdcriteria);
            if (request == null || !Utilities.CheckUserCanEditRequest(CurrentUser.Id, request.Requester.Id))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            else
            {
                var getChangeShiftDetailCriteria = new GetChangeShiftRequestDetailByIdCriteria
                {
                    RequestId = id
                };
                var changeShiftDetail = changeShiftRequestService.GetChangeShiftRequestDetailById(getChangeShiftDetailCriteria);
                var model = new ChangeShiftViewModel
                {
                    RequestId = request.Id,
                    Employee = CurrentUser.NickName,
                    EmployeeId = CurrentUser.Id,
                    FromDate = request.FromDate.Date,
                    ToDate = request.ToDate.Date,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    Remark = request.Remark,
                    ShiftDetails = ConvertChangeShiftDetailToViewModel(changeShiftDetail),
                    VerifierAndApprover = GetVerifierApprover(CurrentUser.Id),
                };

                ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Update(string shiftJson, string shiftDetailJson)
        {
            var criteria = JsonConverter.FromJson<UpdateChangeShiftRequestCriteria>(shiftJson);
            criteria.RequestDetail = JsonConverter.FromJsonArray<ChangeShiftDetailModel>(shiftDetailJson);
            criteria.ModifiedBy = CurrentUser.Id;

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                changeShiftRequestService.SendMailForUpdateSuccessfully(criteria);
            }

            return Json(result);
        }

        public ActionResult Detail(int id)
        {
            var model = new ChangeShiftViewModel();

            var getRequestByIdcriteria = new GetRequestByIdCriteria
            {
                RequestId = id
            };
            RequestEntity request = requestService.GetRequestById(getRequestByIdcriteria);
            var getChangeShiftDetailCriteria = new GetChangeShiftRequestDetailByIdCriteria
            {
                RequestId = id
            };
            IList<ChangeShiftDetailModel> changeShiftDetail = changeShiftRequestService.GetChangeShiftRequestDetailById(getChangeShiftDetailCriteria);

            if (request != null && changeShiftDetail != null)
            {
                var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria { EmpId = request.Requester.Id };
                Employee employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
                model = new ChangeShiftViewModel
                {
                    Employee = employee.NickName,
                    EmployeeId = employee.Id,
                    FromDate = request.FromDate.Date,
                    ToDate = request.ToDate.Date,
                    VerifierId = request.VerifiedBy.Id,
                    ApproverId = request.ApprovedBy.Id,
                    Remark = request.Remark,
                    ShiftDetails = ConvertChangeShiftDetailToViewModel(changeShiftDetail),
                    Status = request.Status,
                    VerifierAndApprover = GetVerifierApprover(employee.Id, request)
                };
            }

            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
        }

        public ActionResult ChangeShiftDetail(int employeeId, DateTime fromDate, DateTime toDate)
        {
            IList<ChangeShiftDetailViewModel> model = GetChangeShiftDetail(employeeId, fromDate, toDate);
            IList<Shift> shifts = shiftService.GetAll(isShowOff: true);
            ViewBag.Shifts = shifts;
            return PartialView("ChangeShiftDetail", model);
        }

        private List<ChangeShiftDetailViewModel> GetChangeShiftDetail(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var changeShiftDetail = new List<ChangeShiftDetailViewModel>();
            var criteria = new GetWorkingCalendarByDateRangeCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                GroupIds = string.Empty,
                EmpId = employeeId
            };
            IList<WorkingCalendarModel> calendars = workingCalendarService.GetWorkingCalendarByDateRange(criteria);
            foreach (WorkingCalendarModel item in calendars)
            {
                var shiftDetail = new ChangeShiftDetailViewModel
                {
                    OnDate = item.WorkDate,
                    Remark = string.Empty,
                    ScheduleStatus = GetScheduleStatus(item),
                    FromShiftId = item.IsOffDay ? -1 : (item.Shift != null ? item.Shift.Id : 0)
                };
                changeShiftDetail.Add(shiftDetail);
            }

            return changeShiftDetail;
        }

        private static string GetScheduleStatus(WorkingCalendarModel model)
        {
            var schedule = string.Empty;
            bool validChangeShift = !model.IsChangeShift && model.Shift != null;

            if (model.IsOffDay)
            {
                schedule = Labels.Off;
            }
            else if (model.IsHoliday)
            {
                schedule = Labels.Holiday;
            }
            else if (!model.IsHoliday && !model.IsLeaveDay && !model.IsKeepInViewDay && validChangeShift)
            {
                schedule = model.Shift.Name;
            }
            else
            {
                schedule = Labels.NA;
            }

            return schedule;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId, RequestEntity request = null)
        {
            var criteria = new GetVerifierAndApproverCriteria
            {
                EmpId = employeeId,
                RequestType = (byte)RequestType.ChangeShift
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

        private decimal GetTotalRequestInMonth(int employeeId)
        {
            var currentDate = DateTime.Now.Date;
            var criteria = new GetNumberRequestInMonthCriteria
            {
                EmployeeId = employeeId,
                RequestType = (int)RequestType.ChangeShift,
                FromDate = new DateTime(currentDate.Year, currentDate.Month, 1),
                ToDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month))
            };
            decimal numberRequests = requestService.GetNumberRequestInMonth(criteria);
            return numberRequests;
        }

        private IList<ChangeShiftDetailViewModel> ConvertChangeShiftDetailToViewModel(IList<ChangeShiftDetailModel> model)
        {
            IList<ChangeShiftDetailViewModel> changeShiftDetails = null;
            if (model != null)
            {
                changeShiftDetails = model.Select(item => new ChangeShiftDetailViewModel
                {
                    OnDate = item.OnDate,
                    ScheduleStatus = item.FromShiftId == -1 ? Labels.Off : item.FromShift,
                    FromShiftId = item.FromShiftId,
                    ToShiftId = item.ToShiftId,
                    Remark = item.Remark
                }).ToList();
            }

            return changeShiftDetails ?? new List<ChangeShiftDetailViewModel>();
        }
    }
}