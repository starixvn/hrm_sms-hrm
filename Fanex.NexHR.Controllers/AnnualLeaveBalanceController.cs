namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class AnnualLeaveBalanceController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IReportService reportService;
        private readonly ICommandService commandService;

        public AnnualLeaveBalanceController()
            : this(null, null, null, null)
        {
        }

        public AnnualLeaveBalanceController(IEmployeeService employeeService, ITeamService teamService, IReportService reportService, ICommandService commandService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.teamService = teamService ?? new TeamService();
            this.reportService = reportService ?? new ReportService();
            this.commandService = commandService ?? new CommandService();
        }

        [HttpPost]
        public JsonResult Adjust(string annualLeaveJson)
        {
            var model = JsonConverter.FromJson<AnnualLeaveBalanceMultiViewModel>(annualLeaveJson);
            var criteria = new AdjustEmployeeAnnualLeaveCriteria
            {
                EmpIds = model.EmpIds,
                Description = model.Description,
                CreatedBy = CurrentUser.Id,
                CreatedDate = DateTime.Now,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                FromDate = model.AffectedFrom
            };
            CommandResult result = commandService.Update(criteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult Reset(int minbalance, string descriptionReset)
        {
            int lastYear = DateTime.Now.AddYears(-1).Year;
            DateTime date = new DateTime(lastYear, 12, 31);

            var criteria = new GetAnnualLeaveBalanceCriteria()
            {
                Year = lastYear,
                EmpId = null,
                GroupIds = null,
                IsLocal = null,
                Role = string.Empty,
                SupervisorID = null
            };
            IList<LeaveSummary> entityList = reportService.GetAnnualLeaveBalanceReport(criteria);
            var resetAnnualLeaveCriteria = new ResetAnnualLeaveCriteria
            {
                CreatedBy = CurrentUser.Id,
                CreatedDate = DateTime.Now,
                Description = descriptionReset,
                AnnualLeaves = entityList.Where(m => m.LeaveBalance > minbalance).ToList(),
                FromDate = date,
                ResetDays = minbalance
            };
            CommandResult result = commandService.Update(resetAnnualLeaveCriteria);

            return Json(result);
        }

        [HRAuthorize(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Index(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? minAnnualBalance = null,
            int? category = null)
        {
            ViewBag.IsShowCloseBalance = ConfigSettings.IsShowCloseBalance;
            bool hasPermissionToEdit = CurrentUser.HasPermission(FunctionKey.AnnualLeaveBalance, Policy.Host);
            ViewBag.HasPermissionToEdit = hasPermissionToEdit;

            if (!employeeId.HasValue && !hasPermissionToEdit)
            {
                employeeId = CurrentUser.Id;
            }

            var model = new LeaveSummaryViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                MinAnnualBalance = minAnnualBalance.HasValue ? minAnnualBalance.Value : 0,
                Category = category
            };

            return Index(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            ViewBag.IsShowCloseBalance = ConfigSettings.IsShowCloseBalance;

            var criteria = new GetAnnualLeaveBalanceCriteria()
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.TeamIds,
                IsLocal = (byte?)model.Category,
                Role = string.Empty,
                SupervisorID = CurrentUser.Id
            };
            IList<LeaveSummary> entityList = reportService.GetAnnualLeaveBalanceReport(criteria);
            var getGroupCriteria = new GetGroupBySupervisorCriteria()
            {
                SupervisorId = CurrentUser.Id
            };
            IList<Team> teams = new List<Team>();
            teams = teamService.GetBySupervisor(getGroupCriteria);

            ViewBag.Groups = teams;
            ViewBag.TeamIds = model.TeamIds;
            ViewBag.EntityList = entityList;
            ViewBag.MinAnnualBalance = model.MinAnnualBalance;
            ViewBag.Category = model.Category;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(entityList, teams);
            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;

            return PartialView(new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        [ChildActionOnly]
        public PartialViewResult MultipleReset()
        {
            return PartialView(new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        //// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            var model = new AnnualLeaveBalanceViewModel
               {
                   EmployeeId = id,
                   Year = DateTime.Now.Year,
                   ExcludedRequestTrans = true
               };

            var getAnnualLeaveCriteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpID = model.EmployeeId,
                ExcludedRequestTrans = false,
                Year = model.Year
            };

            ViewBag.Data = employeeService.GetAnnualLeaveTransactions(getAnnualLeaveCriteria);

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.ExcludedRequestTrans = !model.ExcludedRequestTrans;

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmployeeId };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var getAnnualLeaveCriteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpID = model.EmployeeId,
                ExcludedRequestTrans = model.ExcludedRequestTrans,
                Year = model.Year
            };
            ViewBag.Data = employeeService.GetAnnualLeaveTransactions(getAnnualLeaveCriteria);

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(int id)
        {
            var getAnnualLeaveTransactionByIdCriteria = new GetAnnualLeaveTransactionByIdCriteria() { LeaveTransId = id };
            AnnualLeaveBalanceModel model = employeeService.GetAnnualLeaveTransaction(getAnnualLeaveTransactionByIdCriteria);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpId };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(FormCollection formCollection, AnnualLeaveBalanceModel model)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpId };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;
            var command = formCollection["command"];

            if (command == Resources.Labels.Delete)
            {
                var deleteAnnualLeaveTransactionCriteria = new DeleteAnnualLeaveTransactionCriteria()
                {
                    LeaveTransId = model.LeaveTransId,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = commandService.Delete(deleteAnnualLeaveTransactionCriteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                // update annual leave here
                var updateAnnualLeaveTransactionCriteria = new UpdateAnnualLeaveTransactionCriteria()
                {
                    LeaveTransId = model.LeaveTransId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = commandService.Update(updateAnnualLeaveTransactionCriteria);
                ViewBag.Updated = result.IsSuccessful;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;

            return View(new AnnualLeaveBalanceModel
            {
                EmpId = id,
                AffectedFrom = DateTime.Now,
            });
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(AnnualLeaveBalanceModel model)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpId };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;

            if (ModelState.IsValid)
            {
                var criteria = new InsertAnnualLeaveTransactionCriteria()
                {
                    EmpId = model.EmpId,
                    RequestId = null,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };
                CommandResult result = commandService.Insert(criteria);
                ViewBag.Inserted = result.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }
    }
}