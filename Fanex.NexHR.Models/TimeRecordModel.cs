// --------------------------------------------------------------------------
// <copyright file="TimeRecordModel.cs" company=".">
// Copyright (c) .. All rights reserved.
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
                var typeName = Labels.None;
                if (InOutType == 0)
                {
                    typeName = Labels.In;
                }
                else if (InOutType == 1)
                {
                    typeName = Labels.Out;
                }

                return typeName;
            } 
        }
    }
}
