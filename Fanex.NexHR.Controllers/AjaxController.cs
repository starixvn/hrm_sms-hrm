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
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Criteria.TimeRecord;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Fanex.NexHR.ViewModels.EmployeeGroup;

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
        private readonly IExtensionNumberService _extensionNumberService;

        public AjaxController()
            : this(null, null, null, null, null, null, null, null)
        {
        }

        public AjaxController(
            IRequestService requestService,
            IHolidayService holidayService,
            IMemoService memoService,
            ITeamService teamService,
            IEmployeeService employeeService,
            ILookupDataService lookupDataService,
            IReportService reportService,
            IExtensionNumberService extensionNumberService)
        {
            _requestService = requestService ?? new RequestService();
            _holidayService = holidayService ?? new HolidayService();
            _memoService = memoService ?? new MemoService();
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _reportService = reportService ?? new ReportService();
            _extensionNumberService = extensionNumberService ?? new ExtensionNumberService();
        }

        [HttpPost]
        public string GetMessageDetailRequest(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            request.RequestType = new RequestTypeModel { Id = requestType };
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

        /// <summary>
        /// Function to process a request.
        /// </summary>
        /// <param name="command">Action process request.</param>
        /// <param name="requestId">Request Id.</param>
        /// <param name="roleId">The Role Id.</param>
        /// <param name="remark">The remark.</param>
        /// <returns>Command Result.</returns>
        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProcessRequest(string command, long requestId, int roleId, string remark = null)
        {
            var criteria = new ProcessRequestCriteria()
            {
                Action = command,
                RequestId = requestId,
                RoleID = roleId,
                Comment = remark,
                ProcessedBy = this.CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            CommandResult result = _requestService.Process(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Process single overtime request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="requestId">The request Id.</param>
        /// <param name="roleId">The role Id.</param>
        /// <returns>The Action Result.</returns>
        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        public JsonResult ProcessSingleOvertime(string command, long requestId, int roleId)
        {
            var processOvertimeCriteria = new ProcessSingleOvertimeCriteria
            {
                CommandProcess = command,
                RequestID = requestId,
                RoleID = roleId,
                ProcessedBy = this.CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            CommandResult commandResult = _requestService.ProcessOvertime(processOvertimeCriteria);
            return commandResult.Code == 0 ? Json(true) : Json(commandResult.Message);
        }

        /// <summary>
        /// Process multiple overtime request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="requestIds">The list request Id.</param>
        /// <returns>The Action Result.</returns>
        [Authorize(Roles = "Verifier,Approver,HR")]
        [HttpPost]
        public JsonResult ProcessMultipleOvertime(string command, string requestIds)
        {
            var criteria = new ProcessMultipleOvertimeCriteria
            {
                CommandProcess = command,
                ListRequestID = requestIds,
                ProcessedBy = this.CurrentUser.Id,
                ProcessedDate = DateTime.Now
            };
            criteria.RoleID = criteria.GetRoleIdForProcessOvertime(command);
            CommandResult commandResult = _requestService.ProcessOvertime(criteria);
            return commandResult.Code == 0 ? Json(true) : Json(commandResult.Message);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = _reportService.ProcessInOut(criteria);
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
                Role = (int)RoleSystem.HR,
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
            var model = JsonConverter.FromJson<AnnualLeaveBalanceMultiViewModel>(annualLeaveJson);
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

                var criteria = new GetAnnualLeaveBalanceCriteria()
                {
                    Year = lastYear,
                    EmpId = null,
                    GroupIds = null,
                    IsLocal = null,
                    Role = string.Empty,
                    SupervisorID = null
                };
                IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(criteria);

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
            var criteria = new DeleteMemoCriteria
            {
                Id = id
            };
            return _memoService.Delete(criteria);
        }

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
            var criteria = new DeleteLookupDataCriteria
            {
                Id = id
            };
            CommandResult commandResult = _lookupDataService.Delete(criteria);
            return commandResult.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool InsertJobTitle(string name, int categoryId)
        {
            var criteria = new InsertLookupDataCriteria
            {
                LookupCode = Constants.JobTitle,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult commandResult = _lookupDataService.Insert(criteria);
            return commandResult.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool UpdatetJobTitle(int id, string name, int categoryId)
        {
            var criteria = new UpdateLookupDataCriteria
            {
                Id = id,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult commandResult = _lookupDataService.Update(criteria);
            return commandResult.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public int ResetPassword(int id)
        {
            CommandResult result = _employeeService.ResetPassword(id, "123456", this.CurrentUser.Id);
            return result.Code;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExtensionNumber(int extId)
        {
            var criteria = new DeleteExtensionNumberMemberCriteria
            {
                ExtensionNumberId = extId
            };
            CommandResult commandResult = _extensionNumberService.DeleteExtensionNumber(criteria);
            return commandResult.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool AddNewExternalGroup(string groupName)
        {
            var criteria = new InsertExternalGroupCriteria
            {
                FullName = groupName
            };
            CommandResult commandResult = _extensionNumberService.AddNewExternalGroup(criteria);
            return commandResult.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExternalId = extId
            };
            CommandResult commandResult = _extensionNumberService.DeleteExternalGroup(criteria);
            return commandResult.Code == 0;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalNumberCriteria
            {
                ExternalId = extId,
                FullName = groupName
            };
            CommandResult commandResult = _extensionNumberService.EditExternalGroup(criteria);
            return commandResult.Code == 0;
        }

        public int UpdatePasswordResetStatus(bool status)
        {
            var criteria = new UpdatePasswordResetStatusCriteria()
            {
                EmpId = this.CurrentUser.Id,
                PasswordReset = status
            };
            CommandResult result = _employeeService.UpdatePasswordResetStatus(criteria);

            return result.Code;
        }
    }
}
