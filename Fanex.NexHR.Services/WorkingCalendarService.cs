// --------------------------------------------------------------------------
// <copyright file="WorkingCalendarService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// WorkingCalendar Service.
    /// </summary>
    public class WorkingCalendarService : IWorkingCalendarService
    {
        private readonly IWorkingCalendarRepository _workingCalendarRepository;

        public WorkingCalendarService()
        {
            this._workingCalendarRepository = new WorkingCalendarRepository();
        }

        public WorkingCalendarService(IWorkingCalendarRepository workingCalendarRepository)
        {
            this._workingCalendarRepository = workingCalendarRepository;
        }

        /// <summary>
        /// Function to get WorkingCalendars By Date Range, groups and employee.
        /// </summary>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">Groups Id.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <returns>List WorkingCalendarModel.</returns>
        public IList<WorkingCalendarModel> GetWorkingCalendarsByDateRange(DateTime fromDate, DateTime toDate, string groupIds, int? employeeId = null)
        {
            var workingCalendarModels = _workingCalendarRepository.GetWorkingCalendarsByDateRange(fromDate, toDate, groupIds, employeeId);

            return workingCalendarModels;
        }

        /// <summary>
        /// Function to get WorkingCalendars By Date Range, groups and employee.
        /// </summary>
        /// <param name="supervisorId">Supervisor Id.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">Groups Id.</param>
        /// <param name="jobTitles">Job titles.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <returns>List WorkingCalendarModel.</returns>
        public IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(
            int supervisorId, 
            DateTime fromDate, 
            DateTime toDate, 
            string groupIds,
            string jobTitles, 
            int? employeeId = null)
        {
            var workingCalendarModels = _workingCalendarRepository.GetWorkingCalendarsForShiftArrange(supervisorId, fromDate, toDate, groupIds, jobTitles, employeeId);

            return workingCalendarModels;
        }

        public bool InsertOrUpdateWorkingCalendars(ScheduleArrangeModel scheduleArrangeModel, out int errorCode, out string message)
        {
            var result = _workingCalendarRepository.InsertOrUpdateWorkingCalendars(scheduleArrangeModel, out errorCode, out message);
            
            return result;
        }

        public IList<ShiftStatisticModel> GetShiftStatistic(
            int supervisorId, 
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isShowWholeCompany,
            byte? isLocal)
        {
            var shiftStatisticModels = _workingCalendarRepository.GetShiftStatistic(supervisorId, fromDate, toDate, groupIds, isShowWholeCompany, isLocal);

            return shiftStatisticModels;
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(System.DateTime date, int id)
        {
            Check.Argument.IsNotNegativeOrZero(id, "id");
            return this._workingCalendarRepository.GetWorkingSchedule(date, id);
        }

        public IList<ViewModels.WorkingCalendarsViewModel> GetWorkingCalendars(
            int? empId,
            DateTime fromDate,
            DateTime toDate,
            string groupId, 
            string jobTitleId,
            byte? isLocal = null)
        {
            var workingCalendarList = this._workingCalendarRepository.GetWorkingCalendars(empId, fromDate, toDate, groupId, jobTitleId, isLocal);

            return workingCalendarList;
        }
    }
}
