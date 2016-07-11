namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;

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

        public AnnualLeaveBalanceController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IReportService reportService,
            ICommandService commandService)
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
                SupervisorId = null
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
            string sectionIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? minAnnualBalance = null,
            int? category = null)
        {
            bool hasPermissionToEdit = CurrentUser.HasPermission(FunctionKey.AnnualLeaveBalance, Policy.Host);
            ViewBag.HasPermissionToEdit = hasPermissionToEdit;

            if (!employeeId.HasValue && !hasPermissionToEdit)
            {
                employeeId = CurrentUser.Id;
            }

            var model = new LeaveSummaryViewModel
            {
                Year = year.HasValue ? year.Value : DateTime.Now.Year,
                SectionIds = sectionIds,
                Employee = employee,
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                MinAnnualBalance = minAnnualBalance.HasValue ? minAnnualBalance.Value : 0,
                Category = category,
                IsShowCloseBalance = ConfigSettings.IsShowCloseBalance
            };

            return Index(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            IList<LeaveSummary> leaveSumaries = GetAnnualLeaveBalance(model);
            model.Sections = GetBySupervisor();
            model.IsShowCloseBalance = ConfigSettings.IsShowCloseBalance;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(leaveSumaries, model.Sections);
            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;
            var model = new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            };
            return PartialView(model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleReset()
        {
            var model = new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            };
            return PartialView(model);
        }

        [HRAuthorize(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(int id)
        {
            var model = new AnnualLeaveBalanceViewModel
               {
                   EmployeeId = id,
                   Year = DateTime.Now.Year,
                   ExcludedRequestTrans = true,
                   AnnualLeaveBalances = GetAnnualLeaveTransactions(id, false, DateTime.Now.Year),
                   Employee = GetEmployeeById(id)
               };

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.ExcludedRequestTrans = !model.ExcludedRequestTrans;
            model.Employee = GetEmployeeById(model.EmployeeId);
            model.AnnualLeaveBalances = GetAnnualLeaveTransactions(model.EmployeeId, model.ExcludedRequestTrans, model.Year);

            return View(model);
        }

        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(int id)
        {
            var criteria = new GetAnnualLeaveTransactionByIdCriteria()
            {
                LeaveTransId = id
            };
            AnnualLeaveTransactionViewModel model = employeeService.GetAnnualLeaveTransaction(criteria);
            ViewBag.Employee = GetEmployeeById(model.EmpId);
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(FormCollection formCollection, AnnualLeaveTransactionViewModel model)
        {
            ViewBag.Employee = GetEmployeeById(model.EmpId);
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;

            if (formCollection["command"].Equals(Labels.Delete))
            {
                return Delete(model);
            }
            else
            {
                return Edit(model);
            }
        }

        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(int id)
        {
            ViewBag.Employee = GetEmployeeById(id);
            ViewBag.LocalLeaveTypes = Utilities.LocalLeaveTypes;

            var model = new AnnualLeaveTransactionViewModel
            {
                EmpId = id,
                AffectedFrom = DateTime.Now,
            };
            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(AnnualLeaveTransactionViewModel model)
        {
            ViewBag.Employee = GetEmployeeById(model.EmpId);
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
                    CreatedBy = CurrentUser.Id
                };
                CommandResult result = commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        private IList<LeaveSummary> GetAnnualLeaveBalance(LeaveSummaryViewModel model)
        {
            var criteria = new GetAnnualLeaveBalanceCriteria()
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.SectionIds,
                IsLocal = (byte?)model.Category,
                Role = string.Empty,
                SupervisorId = CurrentUser.Id
            };
            IList<LeaveSummary> leaveSumaries = reportService.GetAnnualLeaveBalanceReport(criteria);
            return leaveSumaries;
        }

        private IList<Team> GetBySupervisor()
        {
            var criteria = new GetGroupBySupervisorCriteria()
            {
                SupervisorId = CurrentUser.Id
            };
            IList<Team> teams = teamService.GetBySupervisor(criteria);

            return teams;
        }

        private Employee GetEmployeeById(int empId)
        {
            var criteria = new GetEmployeeByIdCriteria()
            {
                EmpId = empId
            };

            return employeeService.GetEmployeeById(criteria);
        }

        private IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(int empId, bool excludedRequest, int year)
        {
            var criteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpID = empId,
                ExcludedRequestTrans = false,
                Year = DateTime.Now.Year
            };

            return employeeService.GetAnnualLeaveTransactions(criteria);
        }

        private ActionResult Edit(AnnualLeaveTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
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

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        private ActionResult Delete(AnnualLeaveTransactionViewModel model)
        {
            var criteria = new DeleteAnnualLeaveTransactionCriteria()
            {
                LeaveTransId = model.LeaveTransId,
                ModifiedDate = DateTime.Now,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = commandService.Delete(criteria);
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
    }
}