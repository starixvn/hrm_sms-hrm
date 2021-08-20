namespace Fanex.NexHR.Controllers
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
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.AnnualLeaveBalance;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class AnnualLeaveBalanceController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IReportService reportService;
        private readonly IBaseService baseService;
        private readonly IAnnualLeaveBalanceService annualLeaveBalanceService;

        public AnnualLeaveBalanceController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IReportService reportService,
            IBaseService baseService,
            IAnnualLeaveBalanceService annualLeaveBalanceService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.reportService = reportService;
            this.baseService = baseService;
            this.annualLeaveBalanceService = annualLeaveBalanceService;
        }

        [HRMPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Index(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? category = null)
        {
            ViewBag.HasPermissionToEdit = sessionService.HasPermissionOnFunction(FunctionKey.AnnualLeaveBalance, Policy.Host);
            var model = new LeaveSummaryViewModel
            {
                Year = year ?? DateTime.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId ?? -1,
                Category = category,
                IsShowCloseBalance = ConfigSettings.IsShowCloseBalance
            };

            return Index(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            model.Teams = teamService.GetBySupervisor(sessionService.Employee.Id);
            model.TeamIds = !string.IsNullOrWhiteSpace(model.TeamIds) ? model.TeamIds : string.Join(",", model.Teams.Select(x => x.Id));
            model.IsShowCloseBalance = ConfigSettings.IsShowCloseBalance;
            model.LeaveSumaries = GetAnnualLeaveBalance(model);

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult Adjust(string annualLeaveJson)
        {
            var model = JsonConverter.FromJson<AnnualLeaveBalanceMultiViewModel>(annualLeaveJson);
            var annualLeave = new AdjustAnnualLeaveBalance
            {
                EmployeeIds = model.EmpIds,
                Description = model.Description,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                AffectedFrom = model.AffectedFrom
            };
            CommandResult commandResult = annualLeaveBalanceService.Adjust(sessionService.Employee.Id, annualLeave);

            return Json(commandResult);
        }

        [HttpPost]
        public JsonResult Reset(int minbalance, string descriptionReset)
        {
            int lastYear = DateTime.Now.AddYears(-1).Year;
            var date = new DateTime(lastYear, 12, 31);
            var teams = teamService.GetBySupervisor(sessionService.Employee.Id);
            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = lastYear,
                EmpId = null,
                TeamIds = string.Join(",", teams.Select(x => x.Id)),
                IsLocal = null,
                Role = string.Empty,
                SupervisorId = null
            };
            IList<LeaveSummary> entityList = reportService.GetAnnualLeaveBalanceReport(criteria);
            var resetAnnualLeaveCriteria = new ResetAnnualLeaveCriteria
            {
                ActorId = sessionService.Employee.Id,
                Description = descriptionReset,
                AnnualLeaves = entityList.Where(m => m.LeaveBalance > minbalance).ToList(),
                FromDate = date,
                ResetDays = minbalance
            };
            CommandResult result = baseService.ExecuteNonQuery(resetAnnualLeaveCriteria);

            return Json(result);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
            var model = new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            };
            return PartialView("MultipleInsert", model);
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

        [HRMPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(int id)
        {
            var model = new AnnualLeaveBalanceViewModel
            {
                EmployeeId = id,
                Year = DateTime.Now.Year,
                ExcludedRequestTrans = true,
                AnnualLeaveBalances = GetAnnualLeaveTransactions(id, false, DateTime.Now.Year),
                Employee = employeeService.GetEmployeeById(id)
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.Employee = employeeService.GetEmployeeById(model.EmployeeId);
            model.AnnualLeaveBalances = GetAnnualLeaveTransactions(model.EmployeeId, !model.ExcludedRequestTrans, model.Year);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(int id)
        {
            AnnualLeaveTransactionViewModel model = employeeService.GetAnnualLeaveTransaction(id);
            ViewBag.Employee = employeeService.GetEmployeeById(model.EmpId);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(FormCollection formCollection, AnnualLeaveTransactionViewModel model)
        {
            ViewBag.Employee = employeeService.GetEmployeeById(model.EmpId);
            if (formCollection["command"].Equals(Labels.Delete))
            {
                return Delete(model);
            }

            return Edit(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(int id)
        {
            ViewBag.Employee = employeeService.GetEmployeeById(id);
            var model = new AnnualLeaveTransactionViewModel
            {
                EmpId = id,
                AffectedFrom = DateTime.Now,
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(AnnualLeaveTransactionViewModel model)
        {
            ViewBag.Employee = employeeService.GetEmployeeById(model.EmpId);
            if (ModelState.IsValid)
            {
                var criteria = new InsertAnnualLeaveTransactionCriteria
                {
                    EmpId = model.EmpId,
                    RequestId = null,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTime.Now,
                    CreatedBy = sessionService.Employee.Id
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
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
            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                TeamIds = model.TeamIds,
                IsLocal = (byte?)model.Category,
                Role = string.Empty,
                SupervisorId = sessionService.Employee.Id
            };
            IList<LeaveSummary> leaveSumaries = reportService.GetAnnualLeaveBalanceReport(criteria);
            return leaveSumaries;
        }

        private IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(int empId, bool excludedRequest, int year)
        {
            var criteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpId = empId,
                ExcludedRequestTrans = excludedRequest,
                Year = year
            };

            return employeeService.GetAnnualLeaveTransactions(criteria);
        }

        private ActionResult Edit(AnnualLeaveTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateAnnualLeaveTransactionCriteria
                {
                    LeaveTransId = model.LeaveTransId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = sessionService.Employee.Id
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

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
            var criteria = new DeleteAnnualLeaveTransactionCriteria
            {
                LeaveTransId = model.LeaveTransId,
                ModifiedDate = DateTime.Now,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

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