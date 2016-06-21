// --------------------------------------------------------------------------
// <copyright file="OvertimeTypeModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.5">Created</change>
// </history>

namespace Fanex.NexHR.Models.Overtime
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Overtime Type.
    /// </summary>
    public class OvertimeTypeModel
    {
        public short Id { get; set; }

        public string Name 
        { 
            get 
            {
                if (this.Id == (short)OvertimeType.Normal)
                {
                    return Labels.NormalDay;
                }
                else if (this.Id == (short)OvertimeType.Off)
                {
                    return Labels.OffDay;
                }
                else if (this.Id == (short)OvertimeType.Holiday)
                {
                    return Labels.PublicHoliday;
                }
                else
                {
                    return string.Empty;
                }
            } 
        }
    }
}
