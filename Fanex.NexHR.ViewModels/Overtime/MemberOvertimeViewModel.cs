// --------------------------------------------------------------------------
// <copyright file="MemberOvertimeViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.12.13">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    /// <summary>
    /// MemberOvertime ViewModel.
    /// </summary>
    public class MemberOvertimeViewModel
    {
        public DateTime Month { get; set; }

        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int TeamId { get; set; }

        public int StatusId { get; set; }

        public bool IsHRView { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
