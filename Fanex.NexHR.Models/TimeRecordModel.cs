// --------------------------------------------------------------------------
// <copyright file="TimeRecordModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.1.24">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// TimeRecord Model.
    /// </summary>
    public class TimeRecordModel
    {
        public int RecordTimeId { get; set; }

        public Employee Employee { get; set; }

        public DateTime RecordTime { get; set; }

        public byte? InOutType { get; set; }

        public string GetInOutName 
        { 
            get 
            {
                var inOutLabel = Labels.None;
                if (InOutType == 0)
                {
                    return Labels.In;
                }
                else if (InOutType == 1)
                {
                    return Labels.Out;
                }

                return inOutLabel;
            } 
        }
    }
}
