// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    /// <summary>
    /// AnnualLeaveBalance Controller.
    /// </summary>
    public class AnnualLeaveBalanceController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IReportService _reportService;

        public AnnualLeaveBalanceController()
        {
            _employeeService = new EmployeeService();
            _teamService = new TeamService();
            _reportService = new ReportService();
        }

        public AnnualLeaveBalanceController(IEmployeeService employeeService, ITeamService teamService, IReportService reportService)
        {
            _employeeService = employeeService;
            _teamService = teamService;
            _reportService = reportService ?? new ReportService();
        }

        /// <summary>
        /// ActionResult Index.
        /// </summary>
        /// <param name="teamIds">Team's id.</param>
        /// <param name="employee">Employee name.</param>
        /// <param name="year">Current Year.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="minAnnualBalance">Min Annual Balance.</param>
        /// <param name="category">Employee category.</param>
        /// <returns>Index View.</returns>
        public ActionResult Index(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? minAnnualBalance = null, 
            int? category = null)
        {
            ViewBag.IsShowCloseBalance = ConfigSettings.IsShowCloseBalance;

            if (!employeeId.HasValue 
                && !Roles.IsUserInRole(RoleSystem.HR.ToString())
                && !Roles.IsUserInRole(RoleSystem.Verifier.ToString())
                && !Roles.IsUserInRole(RoleSystem.Approver.ToString()))
            {
                employeeId = this.CurrentUser.Id;
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

        /// <summary>
        /// ActionResult Index.
        /// </summary>
        /// <param name="model">LeaveSummary ViewModel.</param>
        /// <returns>View Index.</returns>
        [HttpPost]
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
                SupervisorID = null
            };
            IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(criteria);

            IList<Team> teams = new List<Team>();
            var getAllTeamCriteria = new GetAllGroupCriteria();
            teams = _teamService.GetAll(getAllTeamCriteria);

            ViewBag.Groups = teams;
            ViewBag.TeamIds = model.TeamIds;
            ViewBag.EntityList = entityList;
            ViewBag.MinAnnualBalance = model.MinAnnualBalance;
            ViewBag.Category = model.Category;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _reportService.GetAnnualLeaveByTeam(entityList, teams);
            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
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

        /// <summary>
        /// ActionResult Employee.
        /// </summary>
        /// <param name="id">Employee's Id.</param>
        /// <returns>View AnnualLeaveBalance of Employee. </returns>
        [Authorize(Roles = "HR")]
        public ActionResult Employee(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);

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

            ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(getAnnualLeaveCriteria);

            return View(model);
        }

        /// <summary>
        /// ActionResult Employee.
        /// </summary>
        /// <param name="model">AnnualLeaveBalance ViewModel.</param>
        /// <returns>View AnnualLeaveBalance of Employee.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.ExcludedRequestTrans = !model.ExcludedRequestTrans;

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmployeeId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var getAnnualLeaveCriteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpID = model.EmployeeId,
                ExcludedRequestTrans = model.ExcludedRequestTrans,
                Year = model.Year
            };
            ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(getAnnualLeaveCriteria);

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var getAnnualLeaveTransactionByIdCriteria = new GetAnnualLeaveTransactionByIdCriteria() { LeaveTransId = id };
            AnnualLeaveBalanceModel model = _employeeService.GetAnnualLeaveTransaction(getAnnualLeaveTransactionByIdCriteria);
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="formCollection">Form Collection.</param>
        /// <param name="model">AnnualLeaveBalance Model.</param>
        /// <returns>View Edit.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Edit(FormCollection formCollection, AnnualLeaveBalanceModel model)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var command = formCollection["command"];

            if (command == Resources.Labels.Delete)
            {
                var deleteAnnualLeaveTransactionCriteria = new DeleteAnnualLeaveTransactionCriteria()
                {
                    LeaveTransId = model.LeaveTransId,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };
                CommandResult result = _employeeService.DeleteAnnualLeaveTransaction(deleteAnnualLeaveTransactionCriteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.ErrorTryAgain);
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
                    ModifiedBy = this.CurrentUser.Id
                };
                CommandResult result = _employeeService.UpdateAnnualLeaveTransaction(updateAnnualLeaveTransactionCriteria);
                ViewBag.Updated = result.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            return View(new AnnualLeaveBalanceModel
            {
                EmpId = id,
                AffectedFrom = DateTime.Now
            });
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">AnnualLeaveBalance Model.</param>
        /// <returns>View Create.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Create(AnnualLeaveBalanceModel model)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            if (ModelState.IsValid)
            {
                var criteria = new InsertAnnualLeaveTransactionCriteria()
                {
                    EmpId = model.EmpId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };
                CommandResult result = _employeeService.InsertAnnualLeaveTransaction(criteria);
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