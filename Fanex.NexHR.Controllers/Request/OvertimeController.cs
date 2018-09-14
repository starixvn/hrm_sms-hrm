namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Core;
    using Core.Enums;
    using Criteria;
    using Criteria.Lookup;
    using Criteria.Overtime;
    using Criteria.Request;
    using Criteria.Supervisor;
    using Criteria.Team;
    using Hangfire;
    using Models;
    using Models.Request;
    using Models.Supervisor;
    using Nexus.RoleManagement.Services;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using Web.Providers;
    using Core.Services;
    using Core.Models;

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Ok.")]
    public class OvertimeController : BaseController
    {
        private readonly ILookupDataService lookupDataService;
        private readonly IEmployeeService employeeService;
        private readonly IRequestService requestService;
        private readonly ITeamService teamService;
        private readonly IOvertimeRequestService overtimeService;
        private readonly ICommandService commandService;

        public OvertimeController()
            : this(null, null, null, null, null, null)
        {
        }

        public OvertimeController(
            IEmployeeService employeeService,
            IRequestService requestService,
            ITeamService teamService,
            IOvertimeRequestService overtimeService,
            ILookupDataService lookupDataService,
            ICommandService commandService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.requestService = requestService ?? new RequestService();
            this.teamService = teamService ?? new TeamService();
            this.overtimeService = overtimeService ?? new OvertimeRequestService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.commandService = commandService ?? new CommandService();
        }

        [HttpGet]
        public ActionResult EmployeeView()
        {
            var model = new OvertimeReportViewModel
            {
                Employee = CurrentUser.NickName,
                EmpId = CurrentUser.Id,
                Year = DateTime.Now.Year
            };
            return EmployeeView(model);
        }

        [HttpPost]
        public ActionResult EmployeeView(OvertimeReportViewModel model)
        {
            if (model != null)
            {
                var criteria = new GetOvertimeReportInYearCriteria
                {
                    EmployeeId = model.EmpId,
                    Year = model.Year
                };
                IList<RequestEntity> entityList = overtimeService.GetOvertimeReportInYear(criteria);
                model.TotalRequest = entityList;
            }

            return View(model);
        }

        public ActionResult ViewDetail(DateTime requestDate, int empId)
        {
            var criteria = new GetOvertimeRequestByMonthCriteria
            {
                EmpId = empId,
                Month = requestDate
            };

            OvertimeRequest overtime = overtimeService.GetOvertimeRequestByMonth(criteria);
            var model = new MyOverTimeViewModel
            {
                OvertimeRequest = overtime.OvertimeDaily,
                EmpId = empId,
                Month = requestDate,
                Status = Utilities.GetCurrentStatusName(overtime.CurrentStatus),
                Employee = overtime.Employee.DisplayName
            };

            return View(model);
        }

        public ActionResult Update(DateTime requestDate, int empId)
        {
            MyOverTimeViewModel model = new MyOverTimeViewModel();
            var criteria = new GetOvertimeRequestByMonthCriteria
            {
                EmpId = empId,
                Month = requestDate
            };

            OvertimeRequest overtime = overtimeService.GetOvertimeRequestByMonth(criteria);
            if (overtime != null)
            {
                model.OvertimeRequest = overtime.OvertimeDaily;
                model.VerifierId = overtime.VerifiedBy != null ? overtime.VerifiedBy.Id : 0;
                model.ApproverId = overtime.ApprovedBy != null ? overtime.ApprovedBy.Id : 0;
                model.EmpId = empId;
                model.Status = Utilities.GetCurrentStatusName(overtime.CurrentStatus);
                model.Employee = overtime.Employee.DisplayName;
                model.Month = requestDate;
                model.VerifierAndApprover = GetVerifierApprover(empId);

                ViewBag.WorkingHours = overtime.Employee.WorkingHours;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Update(string overtimeJson, string overtimeDetailsJson)
        {
            var model = JsonConverter.FromJson<MyOverTimeViewModel>(overtimeJson);
            var overtimeDetailModel = JsonConverter.FromJsonArray<OvertimeDaily>(overtimeDetailsJson).ToList();
            var criteria = new UpdateOvertimeRequestCriteria
            {
                EmployeeId = model.EmpId,
                ProcessedBy = CurrentUser.Id,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                OvertimeRequest = overtimeDetailModel.Where(x => x.TotalWorkHours > 0).ToList()
            };
            CommandResult result = commandService.Update(criteria);
            return Json(result);
        }

        public ActionResult GetActingEmployees(string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = CurrentUser.Id,
                Role = (int)RoleSystem.ActingSupervisor,
                RequestType = null,
                Query = string.IsNullOrWhiteSpace(query) ? string.Empty : query.Trim(),
                IncludeLoginUser = true
            };

            IList<SupervisorNameModel> employees = employeeService.GetSupervisorByQuery(criteria);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime, FunctionKey.ProcessOvertime)]
        public JsonResult ProcessSingle(string command, long requestId, int roleId)
        {
            var criteria = new ProcessSingleOvertimeCriteria
            {
                CommandProcess = command,
                RequestId = requestId,
                RoleId = roleId,
                ProcessedBy = CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            CommandResult result = commandService.Update(criteria);
            return Json(result);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime, FunctionKey.ProcessOvertime)]
        public JsonResult ProcessMultiple(string command, string requestIds)
        {
            var criteria = new ProcessMultipleOvertimeCriteria
            {
                CommandProcess = command,
                ListRequestId = requestIds,
                ProcessedBy = this.CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            criteria.RoleId = ProcessMultipleOvertimeCriteria.GetRoleIdForProcessOvertime(command);
            CommandResult result = commandService.Update(criteria);
            return Json(result);
        }

        [HRAuthorize(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
        public ActionResult MemberOvertime()
        {
            var memberOvertimeViewModel = GetMemberOvertimeViewModel();
            memberOvertimeViewModel.IsHRView = false;

            return View(memberOvertimeViewModel);
        }

        [HRAuthorize(Policy.Host, FunctionKey.ProcessOvertime)]
        public ActionResult EmployeeOvertime()
        {
            var employeeOvertimeViewModel = GetMemberOvertimeViewModel();
            employeeOvertimeViewModel.IsHRView = true;

            return View("MemberOvertime", employeeOvertimeViewModel);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HRAuthorize(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
        public ActionResult MemberOvertimeViewByLeader(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            DateTime monthDefault = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var criteria = new GetMemberOvertimeByLeaderCriteria
            {
                SupervisorId = CurrentUser.Id,
                FromDate = (month.HasValue) ? month.Value : monthDefault,
                ToDate = (month.HasValue) ? month.Value : monthDefault,
                EmployeeId = employeeId,
                GroupIds = teamIds,
                Status = statusId
            };
            IList<OvertimeRequest> memberOvertimes = overtimeService.GetMemberOvertimeByLeader(criteria);

            ViewBag.CurrentUserId = CurrentUser.Id;
            ViewBag.HasVerfifyPermision = CurrentUser.HasPermission(FunctionKey.VerifyOvertime, Policy.Host);
            ViewBag.HasApprovePermision = CurrentUser.HasPermission(FunctionKey.ApproveOvertime, Policy.Host);

            return View(memberOvertimes);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HRAuthorize(Policy.Host, FunctionKey.ProcessOvertime)]
        public ActionResult MemberOvertimeViewByHR(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            DateTime monthDefault = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var criteria = new GetMemberOvertimeCriteria
            {
                SupervisorId = CurrentUser.Id,
                FromDate = (month.HasValue) ? month.Value : monthDefault,
                ToDate = (month.HasValue) ? month.Value : monthDefault,
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
            ViewBag.CurrentUserId = CurrentUser.Id;
            return View(overtime);
        }

        [HRAuthorize(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult YearlyReport()
        {
            IList<Team> sections = teamService.GetAll(new GetAllGroupCriteria());
            var model = new YearlyOvertimeReportViewModel
            {
                Year = DateTime.Now.Year,
                Sections = sections
            };

            return View(model);
        }

        [HRAuthorize(Policy.Read, FunctionKey.ReportHome)]
        public ActionResult YearlyOvertimeReportDetails(YearlyOvertimeReportViewModel model)
        {
            ViewBag.Year = model.Year;
            ViewBag.Calendar = model.Calendar;
            IList<OvertimeAnnualReportViewModel> listEntity = null;

            if (model.Calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorId = CurrentUser.Id,
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

        [HRAuthorize(Policy.Read, FunctionKey.ProcessOvertime)]
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

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.ProcessOvertime)]
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

        [HRAuthorize(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult OTExcelReport()
        {
            IList<Team> sections = teamService.GetAll(new GetAllGroupCriteria());

            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> listJobTitle = lookupDataService.GetAll(getAllJobTitleCriteria);
            IList<SelectListItem> jobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString()
            }).ToList();

            var model = new OvertimeDetailReportViewModel
            {
                FromDate = DateTime.Now.AddMonths(-1),
                ToDate = DateTime.Now,
                Sections = sections,
                JobTitles = jobTitles
            };

            return View(model);
        }

        [HRAuthorize(Policy.Read, FunctionKey.ProcessOvertime)]
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
        [HRAuthorize(Policy.Read, FunctionKey.ProcessOvertime)]
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
        [HRAuthorize(Policy.Host, FunctionKey.ProcessOvertime)]
        public void ReminderClaimOvertime(string subject, string content)
        {
            BackgroundJob.Enqueue(() => overtimeService.SendMailToReminderClaimOvertime(subject, content));
        }

        [NonAction]
        private MemberOvertimeViewModel GetMemberOvertimeViewModel()
        {
            var month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(getAllGroupCriteria);
            var memberOvertimeViewModel = new MemberOvertimeViewModel
            {
                Teams = teams,
                Month = month
            };

            return memberOvertimeViewModel;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId)
        {
            var criteria = new GetVerifierAndApproverCriteria
            {
                EmpId = employeeId,
                RequestType = (byte)RequestType.Overtime
            };
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(criteria);
            return supervisor;
        }
    }
}