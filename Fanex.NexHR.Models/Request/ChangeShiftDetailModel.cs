// --------------------------------------------------------------------------
// <copyright file="ChangeShiftDetailModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.1">Created</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using System;

    /// <summary>
    /// ChangeShiftDetail Model.
    /// </summary>
    public class ChangeShiftDetailModel
    {
        public DateTime OnDate { get; set; }

        public int FromShiftId { get; set; }

        public string FromShift { get; set; }

        public int ToShiftId { get; set; }

        public string ToShift { get; set; }

        public string Remark { get; set; }
    }
}
