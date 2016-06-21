// --------------------------------------------------------------------------
// <copyright file="ITotalWorkingHourMonthlyExporting.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2014.1.15">Created</change>
// </history>

namespace Fanex.NexHR.Services.ExportExcel
{
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Interface Total Working Hour Monthly Exporting.
    /// </summary>
    public interface ITotalWorkingHourMonthlyExporting
    {
        HSSFWorkbook ExportTotalWorkingHourMonthly();
    }
}
