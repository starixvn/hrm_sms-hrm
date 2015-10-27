// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Report;

    /// <summary>
    /// Report Controller API.
    /// </summary>
    public class ReportController : ApiController
    {
        private readonly IReportService _reportService;
        private readonly ITeamService _teamService;

        public ReportController()
        {
            _reportService = new ReportService();
            _teamService = new TeamService();
        }

        public ReportController(IReportService reportService, ITeamService teamService)
        {
            _reportService = reportService;
            _teamService = teamService;
        }

        /// <summary>
        /// Function to get list employee attendance.
        /// </summary>
        /// <param name="model">EmployeeAttendance ViewModel.</param>
        /// <returns>EmployeeAttendanceModel List.</returns>
        public IList<EmployeeAttendanceModel> GetEmployeeAttendances([FromUri]EmployeeAttendanceViewModel model)
        {
            var criteria = new GetEmployeeAttendanceCriteria()
            {
                SupervisorId = model.SupervisorId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                AttendanceType = model.AttendanceType,
                EmpId = model.EmpId,
                Groupids = model.GroupIds,
                ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
                IsLocal = model.IsLocal
            };
            var employeeAttendance = _reportService.GetEmployeeAttendanceReport(criteria);

            return employeeAttendance;
        }

        /// <summary>
        /// Method View TimeSheet.
        /// </summary>
        /// <param name="model">TimesheetViewModel model.</param>
        /// <returns>List of EmployeeAttendanceModel.</returns>
        [HttpGet]
        [HttpPost]
        public IList<EmployeeAttendanceModel> Timesheet([FromUri]TimesheetViewModel model)
        {
            var fromDate = new DateTime(model.Year, model.Month, 1);
            var toDate = new DateTime();
            
            if (DateTime.Now.Month == model.Month && DateTime.Now.Year == model.Year)
            {
                toDate = DateTime.Now.Date;
            }
            else
            {
                toDate = new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month));
            }

            var criteria = new GetEmployeeAttendanceCriteria()
            {
                Fromtime = fromDate,
                Totime = toDate,
                AttendanceType = model.TimeSheetType,
                EmpId = model.EmpId,
                Groupids = string.Empty,
                ExcludeUserDelete = 1,
                IsLocal = null
            };
            var timesheets = _reportService.GetEmployeeAttendanceReport(criteria);
            return timesheets;
        }

        /// <summary>
        /// Method Get Group Extension.
        /// </summary>
        /// <param name="isInternal">Check is Internal or External group.</param>
        /// <param name="groupId">Group's id.</param>
        /// <returns>List of SelectListItem.</returns>
        [HttpGet]
        [HttpPost]
        public IList<System.Web.Mvc.SelectListItem> GetGroupExtension(bool isInternal, int? groupId)
        {
            if (isInternal)
            {
                var getAllGroupCriteria = new GetAllGroupCriteria();
                IList<Team> data = _teamService.GetAll(getAllGroupCriteria);
                var selectList = data.Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                return selectList;
            }
            else
            {
                var getGroupInfoCriteria = new GetExtensionNumberGroupInfoCriteria
                {
                    IsInternal = isInternal,
                    GroupID = groupId
                };
                IEnumerable<ExtensionNoModel> data = _reportService.GetAllExtensionGroupInfo(getGroupInfoCriteria);
                var selectList = data.Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
                return selectList;
            }
        }
    }
}
