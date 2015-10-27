// --------------------------------------------------------------------------
// <copyright file="EmployeeLateInEarlyOutModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.27">Created</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using System;

    public class EmployeeLateInEarlyOutModel
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }
    }
}
