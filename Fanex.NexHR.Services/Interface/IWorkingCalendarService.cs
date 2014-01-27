// --------------------------------------------------------------------------
// <copyright file="IWorkingCalendarService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IWorkingCalendarService
    {
        IList<WorkingCalendarModel> GetWorkingCalendarsByDateRange(DateTime fromDate, DateTime toDate, string groupIds, int? employeeId = null);

        IList<WorkingCalendarModel> GetWorkingCalendarsForShiftArrange(
            int supervisorId, 
            DateTime fromDate, 
            DateTime toDate, 
            string groupIds, 
            string jobTitles, 
            int? employeeId = null);

        bool InsertOrUpdateWorkingCalendars(ScheduleArrangeModel scheduleArrangeModel, out int errorCode, out string message);

        IList<ShiftStatisticModel> GetShiftStatistic(int supervisorId, DateTime fromDate, DateTime toDate, string groupIds, bool isShowWholeCompany, byte? isLocal);

        IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(int? empId, DateTime fromDate, DateTime toDate, string groupId, string jobTitleId, byte? isLocal = null);
    }
}
