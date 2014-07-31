// -----------------------------------------------------------------------
// <copyright file="IOvertimeRequestRepository.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Interface Overtime Request Repository.
    /// </summary>
    public interface IOvertimeRequestRepository
    {
        IList<RequestEntity> GetOvertimeReportInYear(ICriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReport(ICriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(ICriteria criteria);

        RequestEntity GetOvertimeRequestById(ICriteria criteria);

        IList<OvertimeRequest> GetMemberOvertimeDaily(int supervisorId, string roles, int employeeId, DateTime month);

        IList<RequestEntity> GetMemberOvertimeMonthly(int supervisorId, string roles, DateTime month, int? employeeId, string teamIds, string status);

        IList<OvertimeRequest> OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups, byte? isLocal = null);

        IList<OvertimeRequest> GetOvertimeByRequestIds(string requestIds);

        IList<OvertimeExcelReport> OvertimeRecord(DateTime requestDate, int calendar, int? empId, out int errorCode, out string message); 
    }
}
