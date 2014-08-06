// --------------------------------------------------------------------------
// <copyright file="OvertimeReportViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// Overtime Report ViewModel.
    /// </summary>
    public class OvertimeReportViewModel
    {
        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmpId { get; set; }

        public IList<RequestEntity> TotalRequest { get; set; }
    }
}
