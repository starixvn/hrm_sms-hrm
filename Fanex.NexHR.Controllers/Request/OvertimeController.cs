namespace Fanex.NexHR.Controllers.Request
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Overtime;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

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
            var model = new OvertimeReportViewModel()
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
            var model = new MyOverTimeViewModel()
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
                model.VerifierAndApprover = GetVerifierApprover(CurrentUser.Id);

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
            var criteria = new GetEmployeeSupervisorByQueryCriteria()
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

        [Authorize(Roles = "Verifier, Approver")]
        public ActionResult MemberOvertime()
        {
            var memberOvertimeViewModel = GetMemberOvertimeViewModel();
            memberOvertimeViewModel.IsHRView = false;

            return View(memberOvertimeViewModel);
        }

        [Authorize(Roles = "HR")]
        public ActionResult EmployeeOvertime()
        {
            var employeeOvertimeViewModel = GetMemberOvertimeViewModel();
            employeeOvertimeViewModel.IsHRView = true;

            return View("MemberOvertime", employeeOvertimeViewModel);
        }

        [Authorize(Roles = "Verifier, Approver")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult MemberOvertimeViewByLeader(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            if (!month.HasValue)
            {
                month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            var criteria = new GetMemberOvertimeCriteria
            {
                SupervisorID = CurrentUser.Id,
                Role = "2,3",
                FromDate = month.Value,
                ToDate = month.Value,
                EmployeeId = employeeId,
                GroupIds = teamIds,
                Status = statusId
            };
            IList<OvertimeRequest> memberOvertimes = overtimeService.GetMemberOvertime(criteria);

            ViewBag.CurrentUserId = CurrentUser.Id;
            return View(memberOvertimes);
        }

        [Authorize(Roles = "HR")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult MemberOvertimeViewByHR(DateTime? month, int? employeeId, string teamIds, string statusId)
        {
            if (!month.HasValue)
            {
                month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            var criteria = new GetMemberOvertimeCriteria
            {
                SupervisorID = CurrentUser.Id,
                Role = "6",
                FromDate = month.Value,
                ToDate = month.Value,
                EmployeeId = employeeId,
                GroupIds = teamIds,
                Status = statusId
            };
            IList<OvertimeRequest> memberOvertimes = overtimeService.GetMemberOvertime(criteria);

            return View(memberOvertimes);
        }

        public ActionResult MemberDailyOvertime(int requestId, bool isHRView)
        {
            var criteria = new GetOvertimeRequestByIdCriteria()
            {
                RequestId = requestId
            };
            OvertimeRequest overtime = overtimeService.GetOvertimeRequestById(criteria);

            ViewBag.IsHRView = isHRView;
            ViewBag.CurrentUserId = CurrentUser.Id;
            return View(overtime);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.YearlyOvertimeReport)]
        public ActionResult YearlyReport()
        {
            ViewBag.Year = DateTime.Now.Year;
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = teamService.GetAll(getAllGroupCriteria);

            return View();
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.YearlyOvertimeReport)]
        public ActionResult GetYearlyOvertimeReport(int year, string team, int? empId, int? calendar)
        {
            ViewBag.Year = year;
            ViewBag.Calendar = calendar;
            IList<OvertimeAnnualReportViewModel> listEntity = null;

            if (calendar == 1)
            {
                var getYearlyOvertimeReportCriteria = new GetYearlyOvertimeReportCriteria
                {
                    SupervisorID = CurrentUser.Id,
                    Year = year,
                    GroupIds = team,
                    EmployeeId = empId,
                    Status = null,
                    Role = CurrentUser.Roles
                };
                listEntity = overtimeService.GetYearlyOvertimeReport(getYearlyOvertimeReportCriteria);
            }
            else
            {
                var criteria = new GetYearlyOvertimeReportByNormalCalendarCriteria
                {
                    SupervisorID = CurrentUser.Id,
                    Year = year,
                    GroupIds = team,
                    EmployeeId = empId,
                    Status = null,
                    Role = CurrentUser.Roles
                };
                listEntity = overtimeService.GetYearlyOvertimeReportByNormalCalendar(criteria);
            }

            return PartialView("YearlyReportDetails", listEntity);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.OvertimeDetailReport)]
        public ActionResult HRViewDetails(DateTime requestDate, int empId, int? calendar)
        {
            var criteria = new GetOvertimeDailyReportCriteria
            {
                SupervisorID = CurrentUser.Id,
                EmpId = empId,
                Month = Convert.ToDateTime(requestDate).Date,
                Role = CurrentUser.Roles,
                IsNormal = calendar == 0
            };

            OvertimeRequest overtime = overtimeService.GetOvertimeDailyReport(criteria);
            return View(overtime);
        }

        /// [Authorize(Roles = "HR")]
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
            if (calendar == 0)
            {
                var criteria = new GetMonthlyOvertimeReportByNormalCalendarCriteria
                {
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var entityList = overtimeService.GetMonthlyOvertimeReportByNormalCalendar(criteria);
                return PartialView(entityList);
            }
            else
            {
                var date = new DateTime(toDate.Year, toDate.Month, 1);
                var criteria = new GetMemberOvertimeCriteria
                {
                    FromDate = date,
                    ToDate = date
                };
                IList<RequestEntity> entityList = overtimeService.GetMonthlyOvertimeReport(criteria);
                return PartialView(entityList);
            }
        }

        /// [Authorize(Roles = "Verifier, Approver")]
        [HRAuthorize(Policy.Read, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.OvertimeOfEmployee)]
        /// [Authorize(Roles = "Verifier, Approver")]
        [HRAuthorize(Policy.Read, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
            ViewBag.HasVerfifyPermision = CurrentUser.HasPermission(FunctionKey.VerifyOvertime, Policy.Edit);
            ViewBag.HasApprovePermision = CurrentUser.HasPermission(FunctionKey.ApproveOvertime, Policy.Edit);
        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.OvertimeOfEmployee)]
        public ActionResult OTExcelReport()
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = teamService.GetAll(getAllGroupCriteria);
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> listJobTitle = lookupDataService.GetAll(getAllJobTitleCriteria);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString()
            }).ToList();

            return View();
        }

        public ActionResult OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups, byte? isLocal)
        {
            var criteria = new GetOvertimeExcelReportDetailCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                EmployeeID = empId,
                JobTitleId = jobTitle,
                GroupIds = groups,
                IsLocal = isLocal
            };
            var entity = overtimeService.OTExcelReportDetails(criteria);
            return PartialView(entity);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
        public ActionResult CheckInvalidOvertime(string requestIds)
        {
            var criteria = new GetDetailOvertimeByRequestIdsCriteria
            {
                ListRequestID = requestIds
            };
            bool isInvalid = overtimeService.CheckInvalidOvertimes(criteria);

            return Json(isInvalid, JsonRequestBehavior.AllowGet);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
        public void ReminderClaimOvertime(string subject, string content)
        {
            Task.Factory.StartNew(() =>
            {
                overtimeService.SendMailToReminderClaimOvertime(subject, content);
            });
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
