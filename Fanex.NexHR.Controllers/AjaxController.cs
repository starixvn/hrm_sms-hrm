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
            return _employeeService.Delete(id, resignedDate);
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

        [Authorize(Roles = "Verifier,Approver,HR")]
        public JsonResult GetMembersByManager(string keyword, int? groupId = null)
        {
            var role = RolesEnum.HR;
            return Json(
                _employeeService.GetMembersByManager(this.CurrentUser.Id, (short)role, keyword, groupId).Take(10),
                JsonRequestBehavior.AllowGet);
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
            var result = _employeeService.AdjustAnnualLeaves(
                ModelState,
                model.EmpIds,
                model.Description,
                model.AffectedFrom,
                model.DeductDays,
                model.CreditDays,
                DateTime.Now,
                this.CurrentUser.Id);
            if (!result)
            {
                foreach (var item in ModelState.Values)
                {
                    foreach (ModelError error in item.Errors)
                    {
                        return Json(new { success = false, errors = error.ErrorMessage });
                    }
                }
            }

            return Json(result);
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
                return Json(_employeeService.ResetAnnualLeaves(
                    strValue.ToString(),
                    descriptionReset,
                    date,
                    minbalance,
                    DateTime.Now,
                    this.CurrentUser.Id));
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

        [Authorize(Roles = "HR")]
        [HttpPost]
        public int AddNewTeam(TeamViewModel model)
        {
            var errorCode = 0;
            var message = string.Empty;

            return _teamService.AddNewTeam(model, out errorCode, out message);
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
            int errCode;
            string errMsg;
            var result = _teamService.DeleteTeam(id, DateTime.Now, this.CurrentUser.Id, out errCode, out errMsg);

            if (errCode != 0)
            {
                return errMsg;
            }

            return string.Empty;
        }

        public bool UpdateShift(int id, DateTime date, int? shift)
        {
            return _employeeService.UpdateShift(id, date, shift, this.CurrentUser.Id, DateTime.Now);
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
    }
}
