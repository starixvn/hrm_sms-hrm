// --------------------------------------------------------------------------
// <copyright file="IExportExcelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    public interface IExportExcelService
    {
        HSSFWorkbook ExportTotalWorkingHours(IList<TotalWorkingHours> listTotalWorkingHours, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportAttendance(IList<EmployeeAttendanceModel> attendanceList, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year);

        HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList);

        HSSFWorkbook ExportOvertimeReport(IList<OvertimeRequest> requestList, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportYearlyOvertimeReport(IList<OvertimeAnnualReport> requestList, int year, int? calendar);

        HSSFWorkbook ExportTotalWorkingHourMonthly(IList<TotalWorkingHourMonthly> totalWorkingHourMonthly, TotalWorkingHourMonthly firstRecord);

        HSSFWorkbook ExportEmployeeBasicInfo(IList<Employee> employees);

        HSSFWorkbook ExportEmployeeDetailsInfo(IList<Employee> employees);

        HSSFWorkbook ExportEmployeeWorkingCalendar(IList<WorkingCalendarsViewModel> workingCalendars, DateTime fromDate, DateTime toDate);

        HSSFWorkbook ExportEmployeeOvertimeRecord(List<OvertimeExcelReport> overtimes, int? calendar, DateTime fromDate, DateTime toDate);
    }
}