// --------------------------------------------------------------------------
// <copyright file="ITotalWorkingHourMonthlyExporting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2014.1.15">Created</change>
// </history>

namespace Fanex.NexHR.Services.ExcelExporting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ITotalWorkingHourMonthlyExporting
    {
        HSSFWorkbook ExportTotalWorkingHourMonthly();
    }
}
