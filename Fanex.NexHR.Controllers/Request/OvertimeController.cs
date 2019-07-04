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
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Overtime;
    using Hangfire;

    public class OvertimeController : BaseController
    {
        private readonly ILookupDataService lookupDataService;
        private readonly IEmployeeService employeeService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly IOvertimeRequestService overtimeService;
        private readonly IBaseService baseService;

        public OvertimeController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IRequestService requestService,
            ITeamService teamService,
            IOvertimeRequestService overtimeService,
            ILookupDataService lookupDataService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.requestService = requestService;
            this.teamService = teamService;
            this.overtimeService = overtimeService;
            this.lookupDataService = lookupDataService;
            this.baseService = baseService;
        }

        [HttpGet]
        public ActionResult EmployeeView(EmployeeMonthlyOvertimeViewModel model)
        {
            model.Employee = model.Employee ?? sessionService.Employee.Nickname;
            model.EmployeeId = model.EmployeeId <= 0 ? sessionService.Employee.Id : model.EmployeeId;
            model.OvertimeRequests = overtimeService.GetMonthlyRequestByEmployeeId(model.EmployeeId, model.Year);

            return View(model);
        }

        public ActionResult Details(int employeeId, DateTime month)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);

            return View(monthlyRequest);
        }

        public ActionResult Edit(int employeeId, DateTime month)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(employeeId, RequestType.Overtime);

            var model = new EditMonthlyRequestViewModel
            {
                MonthlyRequest = monthlyRequest,
                VerifierId = monthlyRequest.Verifier != null ? monthlyRequest.Verifier.EmployeeId : 0,
                ApproverId = monthlyRequest.Approver != null ? monthlyRequest.Approver.EmployeeId : 0,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
            };

            return View(model);
        }

        [HttpPost]
        public CustomJsonResult Edit(EditMonthlyOvertimeRequest model)
        {
            CommandResult commandResult = overtimeService.EditMonthlyRequest(sessionService.Employee.Id, model);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetActingEmployees(string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = sessionService.Employee.Id,
                Role = (int)RoleSystem.OnBehalfRequest,
                RequestType = (byte)RequestType.Overtime,
                Query = string.IsNullOrWhiteSpace(query) ? string.Empty : query.Trim(),
                IncludeLoginUser = true
            };

            IList<SupervisorNameModel> employees = employeeService.GetSupervisorByQuery(criteria);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public JsonResult ProcessSingle(string command, long requestId, int roleId)
        {
            var criteria = new ProcessSingleOvertimeCriteria
            {
                CommandProcess = command,
                RequestId = requestId,
                RoleId = roleId,
                ProcessedBy = sessionService.Employee.Id,
                ProcessedDate = DateTime.Now
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public JsonResult ProcessMultiple(string command, string requestIds)
        {
            var criteria = new ProcessMultipleOvertimeCriteria
            {
                CommandProcess = command,
                ListRequestId = requestIds,
                ProcessedBy = sessionService.Employee.Id,
                ProcessedDate = DateTime.Now
            };
            criteria.RoleId = ProcessMultipleOvertimeCriteria.GetRoleIdForProcessOvertime(command);
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveOvertime)]
        public ActionResult MemberOvertime()
        {
            var memberOvertimeViewModel = GetMemberOvertimeViewModel();
            memberOvertimeViewModel.IsHRView = false;

            return View(memberOvertimeViewModel);
        }

        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public ActionResult EmployeeOvertime()
        {
            var employeeOvertimeViewModel = GetMemberOvertimeViewModel();
            employeeOvertimeViewModel.IsHRView = true;

            return View("MemberOvertime", employeeOvertimeViewModel);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveOvertime)]
        public ActionResult MemberOvertimeViewByLeader(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            var criteria = new GetMemberOvertimeByLeaderCriteria
            {
                SupervisorId = sessionService.Employee.Id,
                FromDate = month != null ? month.Value : month,
                ToDate = month != null ? month.Value : month,
                EmployeeId = employeeId,
                GroupIds = teamIds,
                Status = statusId
            };
            IList<OvertimeRequest> memberOvertimes = overtimeService.GetMemberOvertimeByLeader(criteria);

            ViewBag.CurrentUserId = sessionService.Employee.Id;
            ViewBag.HasVerfifyPermision = sessionService.HasPermissionOnFunction(FunctionKey.VerifyOvertime, Policy.Host);
            ViewBag.HasApprovePermision = sessionService.HasPermissionOnFunction(FunctionKey.ApproveOvertime, Policy.Host);

            return View(memberOvertimes);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public ActionResult MemberOvertimeViewByHR(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            var criteria = new GetMemberOvertimeCriteria
            {
                SupervisorId = sessionService.Employee.Id,
                FromDate = month != null ? month.Value : month,
                ToDate = month != null ? month.Value : month,
                EmployeeId = employeeId,
                GroupIds = teamIds,
                Status = statusId
            };
            IList<OvertimeRequest> memberOvertimes = overtimeService.GetMemberOvertime(criteria);

            return View(memberOvertimes);
        }

        public ActionResult MemberDailyOvertime(int requestId, bool isHRView)
        {
            var criteria = new GetOvertimeRequestByIdCriteria
            {
                RequestId = requestId
            };
            OvertimeRequest overtime = overtimeService.GetOvertimeRequestById(criteria);

            ViewBag.IsHRView = isHRView;
            ViewBag.CurrentUserId = sessionService.Employee.Id;
            return View(overtime);
        }

        [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult YearlyReport()
        {
            var model = new YearlyOvertimeReportViewModel
            {
                Year = DateTime.Now.Year,
                Sections = teamService.GetAll()
            };

            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult YearlyOvertimeReportDetails(YearlyOvertimeReportViewModel model)
        {
            ViewBag.Year = model.Year;
            ViewBag.Calendar = model.Calendar;
            IList<OvertimeAnnualReportViewModel> listEntity = null;

            if (model.Calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorId = sessionService.Employee.Id,
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                listEntity = overtimeService.GetYearlyOvertimeReport(getYearlyOvertimeReportCriteria);
            }
            else
            {
                var criteria = new GetYearlyOvertimeReportByNormalCalendarCriteria
                {
                    Year = model.Year,
                    GroupIds = model.SectionIds,
                    EmployeeId = model.EmployeeId,
                    Status = null
                };
                listEntity = overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            return PartialView("_YearlyReportDetails", listEntity);
        }

        [HRMPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult HRViewDetails(DateTime requestDate, int empId, int? calendar)
        {
            var criteria = new GetOvertimeDailyReportCriteria
            {
                EmpId = empId,
                Month = Convert.ToDateTime(requestDate).Date,
                IsNormal = calendar == 0
            };

            OvertimeRequest overtime = overtimeService.GetOvertimeDailyReport(criteria);
            return View(overtime);
        }

        [HRMPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult HRMonthlyReport(int month, int year, int? calendar)
        {
            var date = new DateTime(year, month, 1);
            ViewBag.Date = date.ToString("MMM-yyyy");
            ViewBag.Calendar = calendar;

            if (calendar == 0)
            {
                var fromDate = new DateTime(year, month, 1);
                var toDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
            }
            else
            {
                var currentDate = new DateTime(year, month, DateTime.Now.Day);
                var requestDatePreviousOneMonth = currentDate.AddMonths(-1).Date;
                var fromDate = new DateTime(requestDatePreviousOneMonth.Year, requestDatePreviousOneMonth.Month, 10);
                var toDate = new DateTime(year, month, 9);
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
            }

            return View();
        }

        public ActionResult HRMonthlyReportDetails(DateTime fromDate, DateTime toDate, int? calendar)
        {
            ViewBag.Calendar = calendar;
            IList<RequestEntity> entityList;
            if (calendar == 0)
            {
                var criteria = new GetMonthlyOvertimeReportByNormalCalendarCriteria
                {
                    FromDate = fromDate,
                    ToDate = toDate
                };
                entityList = overtimeService.GetMonthlyOvertimeReportByNormalCalendar(criteria);
            }
            else
            {
                var date = new DateTime(toDate.Year, toDate.Month, 1);
                var criteria = new GetMemberOvertimeCriteria
                {
                    FromDate = date,
                    ToDate = date
                };
                entityList = overtimeService.GetMonthlyOvertimeReport(criteria);
            }

            return PartialView("_HRMonthlyReportDetails", entityList);
        }

        [HRMPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult OTExcelReport()
        {
            IList<LookupData> listJobTitle = lookupDataService.GetAll(Constants.JobTitle);
            IList<SelectListItem> jobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString()
            }).ToList();

            var model = new OvertimeDetailReportViewModel
            {
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now,
                Sections = teamService.GetAll(),
                JobTitles = jobTitles
            };

            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult OTExcelReportDetails(OvertimeDetailReportViewModel model)
        {
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = model.EmployeeId,
                JobTitleId = model.JobTitleIds,
                GroupIds = model.SectionIds,
                IsLocal = model.IsLocal
            };
            IList<OvertimeDaily> overtimeDetails = overtimeService.OTExcelReportDetails(criteria).OrderBy(x => x.OvertimeDate).ToList();

            return PartialView("_OTExcelReportDetails", overtimeDetails);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult CheckInvalidOvertime(string requestIds)
        {
            var criteria = new GetDetailOvertimeByRequestIdsCriteria
            {
                ListRequestId = requestIds
            };
            bool isInvalid = overtimeService.CheckInvalidOvertimes(criteria);

            return Json(isInvalid, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public void ReminderClaimOvertime(string subject, string content)
        {
            BackgroundJob.Enqueue(() => overtimeService.SendMailToReminderClaimOvertime(subject, content));
        }

        [NonAction]
        private MemberOvertimeViewModel GetMemberOvertimeViewModel()
        {
            var memberOvertimeViewModel = new MemberOvertimeViewModel
            {
                Teams = teamService.GetAll(),
                StatusId = -1
            };

            return memberOvertimeViewModel;
        }
    }
}