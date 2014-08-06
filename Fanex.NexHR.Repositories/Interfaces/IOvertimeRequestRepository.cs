// --------------------------------------------------------------------------
// <copyright file="IOvertimeRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// Interface Overtime Request Repository.
    /// </summary>
    public interface IOvertimeRequestRepository
    {
        IList<RequestEntity> GetOvertimeReportInYear(ICriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReport(ICriteria criteria);

        IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(ICriteria criteria);

        RequestEntity GetOvertimeRequestById(ICriteria criteria);

        IList<RequestEntity> GetMemberOvertimeMonthly(ICriteria criteria);

        IList<OvertimeExcelReport> OvertimeRecord(ICriteria criteria);
    }
}
