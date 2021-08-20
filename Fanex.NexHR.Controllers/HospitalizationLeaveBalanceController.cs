namespace Fanex.NexHR.Controllers
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
    using Fanex.NexHR.Criteria.HospitalizationLeave;
    using Fanex.NexHR.Models.HospitalizationLeave;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Fanex.NexHR.ViewModels.HospitalizationLeave;

    [HRMPermission(Policy.Read, FunctionKey.HospitalizationLeaveBalance)]
    public class HospitalizationLeaveBalanceController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;

        public HospitalizationLeaveBalanceController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.baseService = baseService;
        }

        public ActionResult Index(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            bool? isLocal = null)
        {
            var model = new HospitalizationLeaveBalanceViewModel
            {
                Year = year ?? DateTime.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId,
                IsLocal = isLocal,
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.HospitalizationLeaveBalance, Policy.Host)
            };

            return Index(model);
        }

        [HttpPost]
        public ActionResult Index(HospitalizationLeaveBalanceViewModel model)
        {
            model.Teams = teamService.GetBySupervisor(sessionService.Employee.Id);
            model.TeamIds = !string.IsNullOrWhiteSpace(model.TeamIds) ? model.TeamIds : string.Join(",", model.Teams.Select(x => x.Id));
            var criteria = new GetSummaryReportCriteria
            {
                Year = model.Year,
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds,
                IsLocal = model.IsLocal,
                SupervisorId = sessionService.Employee.Id
            };
            model.Employees = baseService.GetData<HospitalizationLeaveBalanceSummaryReport>(criteria);

            return View(model);
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

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
        public JsonResult Adjust(AnnualLeaveBalanceMultiViewModel model)
        {
            var criteria = new InsertMultipleTransactionCriteria
            {
                EmployeeIds = model.EmpIds,
                Description = model.Description,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                FromDate = model.AffectedFrom,
                ActorId = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
        public JsonResult Reset(ResetMultipleTransactionCriteria criteria)
        {
            criteria.ActorId = sessionService.Employee.Id;
            criteria.Year = DateTime.Now.AddYears(-1).Year;
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Employee(int id)
        {
            var employee = employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var model = new HospitalizationLeaveTransactionViewModel
            {
                EmployeeId = id,
                Nickname = employee.NickName,
                Year = DateTime.Now.Year,
                IncludedRequestTrans = false,
                Transactions = GetHospitalizationLeaveTransaction(id, DateTime.Now.Year, false)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Employee(HospitalizationLeaveTransactionViewModel model)
        {
            var employee = employeeService.GetEmployeeById(model.EmployeeId);
            model.Nickname = employee.NickName;
            model.Transactions = GetHospitalizationLeaveTransaction(model.EmployeeId, model.Year, model.IncludedRequestTrans);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
        public ActionResult Create(int id)
        {
            var employee = employeeService.GetEmployeeById(id);
            var model = new HospitalizationLeaveTransaction
            {
                EmpId = id,
                Nickname = employee.NickName,
                AffectedFrom = DateTime.Now,
            };
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
        public ActionResult Create(HospitalizationLeaveTransaction model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertTransactionCriteria
                {
                    EmpId = model.EmpId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTime.Now,
                    CreatedBy = sessionService.Employee.Id
                };
                CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

                if (commandResult.IsSuccessful)
                {
                    return RedirectToAction("Employee", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
        public ActionResult Edit(int id)
        {
            var criteria = new GetTransactionByIdCriteria
            {
                TransId = id
            };
            var transaction = baseService.GetDataEntity<HospitalizationLeaveTransaction>(criteria);
            if (transaction == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(transaction);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
        public ActionResult Edit(FormCollection formCollection, HospitalizationLeaveTransaction model)
        {
            if (formCollection["command"].Equals(Labels.Delete))
            {
                return Delete(model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    return Edit(model);
                }
                else
                {
                    return View(model);
                }
            }
        }

        private IEnumerable<HospitalizationLeaveTransaction> GetHospitalizationLeaveTransaction(int employeeId, int year, bool includedRequestTrans = true)
        {
            var criteria = new GetTransactionByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                IncludedRequestTrans = includedRequestTrans,
                Year = year
            };

            return baseService.GetData<HospitalizationLeaveTransaction>(criteria);
        }

        private ActionResult Edit(HospitalizationLeaveTransaction model)
        {
            var criteria = new UpdateTransactionCriteria
            {
                TransId = model.TransId,
                Description = model.Description,
                FromDate = model.AffectedFrom,
                DeductDays = model.DeductDays,
                CreditDays = model.CreditDays,
                ModifiedDate = DateTime.Now,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                ViewBag.Updated = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }

            return View(model);
        }

        private ActionResult Delete(HospitalizationLeaveTransaction model)
        {
            var criteria = new DeleteTransactionCriteria
            {
                TransId = model.TransId,
                ModifiedDate = DateTime.Now,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                return RedirectToAction("Employee", new { id = model.EmpId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
                return View(model);
            }
        }
    }
}