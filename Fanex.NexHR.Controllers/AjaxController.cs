// --------------------------------------------------------------------------
// <copyright file="AjaxController.cs" company="Nexcel Solutions Vietnam">
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
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report.TimeRecord;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Ajax Controller.
    /// </summary>
    [Authorize]
    public class AjaxController : BaseController
    {
        private readonly IRequestService _requestService;
        private readonly IHolidayService _holidayService;
        private readonly IMemoService _memoService;
        private readonly ITeamService _teamService;
        private readonly IEmployeeService _employeeService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IReportService _reportService;

        public AjaxController()
        {
            _requestService = new RequestService();
            _holidayService = new HolidayService();
            _memoService = new MemoService();
            _teamService = new TeamService();
            _employeeService = new EmployeeService();
            _lookupDataService = new LookupDataService();
            _reportService = new ReportService();
        }

        public AjaxController(
            IRequestService requestService,
            IHolidayService holidayService,
            IMemoService memoService,
            ITeamService teamService,
            IEmployeeService employeeService,
            ILookupDataService lookupDataService,
            IReportService reportService)
        {
            _requestService = requestService ?? new RequestService();
            _holidayService = holidayService ?? new HolidayService();
            _memoService = memoService ?? new MemoService();
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _reportService = reportService ?? new ReportService();
        }

        [HttpPost]
        public string GetMessageDetailRequest(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            request.Type = new RequestType { Id = requestType };
            request.FromShift = new Shift { Name = fromShiftCode };
            request.ToShift = new Shift { Name = toShiftCode };

            var date = RequestService.ToRequestMessage(request);
            return date;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteEmployee(int id, DateTime resignedDate)
        {
            var deleteEmployeeCriteria = new DeleteEmployeeCriteria
            {
                EmpId = id,
                ResignedDate = resignedDate
            };
            CommandResult result = _employeeService.Delete(deleteEmployeeCriteria);

            return result.Code == 0;
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        public JsonResult ProcessRequest(string command, long requestId, int roleId, string remark = null)
        {
            string errorMessage;
            var result = _requestService.Process(command, requestId, roleId, this.CurrentUser.Id, remark, out errorMessage);
            return result ? Json(true) : Json(errorMessage);
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        public JsonResult ProcessSingleOvertime(string command, long requestId, int roleId)
        {
            string errorMessage;
            var result = _requestService.ProcessOvertime(command, requestId, roleId, this.CurrentUser.Id, out errorMessage);
            return result ? Json(true) : Json(errorMessage);
        }

        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        public JsonResult ProcessMultipleOvertime(string command, string requestIds)
        {
            string errorMessage;
            var result = _requestService.ProcessOvertime(command, requestIds, this.CurrentUser.Id, out errorMessage);
            return result ? Json(true) : Json(errorMessage);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessInOut(TimeRecordUpdateTypeCriteria model)
        {
            if (model != null)
            {
                model.ModifiedBy = this.CurrentUser.Id;
                model.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = _reportService.ProcessInOut(model);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Employees By Manager.
        /// </summary>
        /// <param name="keyword">Key words.</param>
        /// <param name="groupId">Filter by groups.</param>
        /// <returns>JSON Result.</returns>
        [Authorize(Roles = "Verifier,Approver,HR")]
        public JsonResult GetMembersByManager(string keyword, int? groupId = null)
        {
            var criteria = new GetEmployeeByManagerCriteria()
            {
                ManagerID = this.CurrentUser.Id,
                Role = (int)RolesEnum.HR,
                Keyword = keyword,
                GroupID = groupId,
                ExcludedManager = false
            };
            IList<Employee> employees = _employeeService.GetMembersByManager(criteria);

            return Json(employees.Take(10), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adjust Annual Leaves.
        /// </summary>
        /// <param name="annualLeaveJson">AnnualLeaveBalanceMulti ViewModel in string Format.</param>
        /// <returns>JSON Result AdjustAnnualLeaves.</returns>
        [HttpPost]
        public JsonResult AdjustAnnualLeaves(string annualLeaveJson)
        {
            var model = JsonConverter.JsonExtensionMethod.FromJson<AnnualLeaveBalanceMultiViewModel>(annualLeaveJson);
            var adjustAnnualLeaveCriteria = new AdjustEmployeeAnnualLeaveCriteria
            {
                EmpIds = model.EmpIds,
                Description = model.Description,
                CreatedBy = this.CurrentUser.Id,
                CreatedDate = DateTime.Now,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                FromDate = model.AffectedFrom
            };
            CommandResult result = _employeeService.AdjustAnnualLeaves(adjustAnnualLeaveCriteria);
            if (result.Code != 0)
            {
                return Json(new { success = false, errors = result.Message });
            }

            return Json(true);
        }

        /// <summary>
        /// Reset Annual Leaves.
        /// </summary>
        /// <param name="minbalance">Min balance.</param>
        /// <param name="descriptionReset">Description Reset.</param>
        /// <returns>JSON Result ResetAnnualLeaves.</returns>
        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            if (ModelState.IsValid)
            {
                int lastYear = DateTime.Now.AddYears(-1).Year;
                DateTime date = new DateTime(lastYear, 12, 31);

                IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(
                    lastYear,
                    empId: null,
                    groupIds: null,
                    isLocal: null);

                entityList = entityList.Where(m => m.LeaveBalance > minbalance).ToList();

                string template = "<Employee Id=\"{0}\" LeaveBalance=\"{1}\"/>";
                StringBuilder strValue = new StringBuilder();
                strValue.Append("<ROOT>");
                foreach (var item in entityList)
                {
                    strValue.Append(string.Format(template, item.Employee.Id, item.LeaveBalance));
                }

                strValue.Append("</ROOT>");

                var resetAnnualLeaveCriteria = new ResetAnnualLeaveCriteria
                {
                    CreatedBy = this.CurrentUser.Id,
                    CreatedDate = DateTime.Now,
                    Description = descriptionReset,
                    EmpValue = strValue.ToString(),
                    FromDate = date,
                    ResetDays = minbalance
                };
                CommandResult result = _employeeService.ResetAnnualLeaves(resetAnnualLeaveCriteria);

                return Json(result.Code == 0);
            }
            else
            {
                return Json(-1);
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteHoliday(int id)
        {
            return _holidayService.Delete(id, this.CurrentUser.Id);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteMemo(int id)
        {
            return _memoService.Delete(id);
        }

        /// <summary>
        /// Add new Team.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The error code.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public int AddNewTeam(TeamViewModel model)
        {
            var criteria = new InsertNewGroupCriteria
            {
                Code = model.TeamCode,
                Name = model.TeamName,
                MailGroup = model.MailGroup,
                LeaderId = model.LeaderId,
                AssistantLeaderId = (int)model.AssistantLeaderId,
                CreatedDate = DateTime.Now,
                CreatedBy = this.CurrentUser.Id
            };
            CommandResult commandResult = _teamService.AddNewTeam(criteria);
            return commandResult.Code;
        }

        /// <summary>
        /// Delete team.
        /// </summary>
        /// <param name="id">The Team Id.</param>
        /// <returns>Message result delete.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public string DeleteTeam(int id)
        {
            var criteria = new DeleteGroupCriteria
            {
                GroupID = id,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };
            CommandResult commandResult = _teamService.DeleteTeam(criteria);

            if (commandResult.Code != 0)
            {
                return commandResult.Message;
            }

            return string.Empty;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteJobTitle(int id)
        {
            return _lookupDataService.Delete(id);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool InsertJobTitle(string name)
        {
            return _lookupDataService.Insert(name, LookupCodeName.JobTitle);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool UpdatetJobTitle(int id, string name)
        {
            return _lookupDataService.Update(id, name);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public int ResetPassword(int id)
        {
            return _employeeService.ResetPassword(id, "123456");
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExtensionNumber(int extId)
        {
            return _reportService.DeleteExtensionNumber(extId);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool AddNewExternalGroup(string groupName)
        {
            return _reportService.AddNewExternalGroup(groupName);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExternalGroup(int extId)
        {
            return _reportService.DeleteExternalGroup(extId);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool EditExternalGroup(int extId, string groupName)
        {
            return _reportService.EditExternalGroup(extId, groupName);
        }

        public int UpdatePasswordResetStatus(bool status)
        {
            return _employeeService.UpdatePasswordResetStatus(this.CurrentUser.Id, status);
        }
    }
}
