// --------------------------------------------------------------------------
// <copyright file="OvertimeAnnualReportViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Overtime
{
    /// <summary>
    /// Overtime Annual Report.
    /// </summary>
    public class OvertimeAnnualReportViewModel
    {
        public int EmpId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Team { get; set; }

        public string TeamCode { get; set; }

        public float Jan { get; set; }

        public float Feb { get; set; }

        public float Mar { get; set; }

        public float Apr { get; set; }

        public float May { get; set; }

        public float Jun { get; set; }

        public float Jul { get; set; }

        public float Aug { get; set; }

        public float Sep { get; set; }

        public float Oct { get; set; }

        public float Nov { get; set; }

        public float Dec { get; set; }

        public float Total { get; set; }
    }
}
