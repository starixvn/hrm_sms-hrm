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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report Controller API.
    /// </summary>
    public class ReportController : ApiController
    {
        private readonly IReportService _reportService;
        private readonly ITeamService _teamService;
        private readonly IEmployeeService _employeeService;

        public ReportController()
        {
            _reportService = new ReportService();
            _teamService = new TeamService();
            _employeeService = new EmployeeService();
        }

        public ReportController(IReportService reportService, ITeamService teamService, IEmployeeService employeeService)
        {
            _reportService = reportService;
            _teamService = teamService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Function to get list employee attendance.
        /// </summary>
        /// <param name="model">EmployeeAttendance ViewModel.</param>
        /// <returns>EmployeeAttendanceModel List.</returns>
        public IList<EmployeeAttendanceModel> GetEmployeeAttendances([FromUri]EmployeeAttendanceViewModel model)
        {
            if (model.CurrentUserId > 0)
            {
                var employee = _employeeService.GetEmployeeById(model.CurrentUserId);
                if (employee.IsMember())
                {
                    model.GroupIds = employee.Team.Id.ToString();
                }
            }
            else if (model.GroupIds == null)
            {
                model.GroupIds = string.Empty;
            }

            var employeeAttendance = _reportService.GetEmployeeAttendanceReport(
                model.FromDate, 
                model.ToDate, 
                model.Type, 
                model.EmpId, 
                model.GroupIds, 
                model.ExcludeDeletedUsers ? 1 : 0,
                model.IsLocal);

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

            var timesheets = _reportService.GetEmployeeAttendanceReport(fromDate, toDate, model.Type, model.EmpId);
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
                var data = _teamService.GetAll();
                var selectList = data.Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                return selectList;
            }
            else
            {
                var data = _reportService.GetAllExtensionGroupInfo(isInternal, groupId);
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
