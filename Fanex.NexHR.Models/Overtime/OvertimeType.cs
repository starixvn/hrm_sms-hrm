// --------------------------------------------------------------------------
// <copyright file="OvertimeType.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.5">Created</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Overtime Type.
    /// </summary>
    public class OvertimeType
    {
        public short Id { get; set; }

        public string Name 
        { 
            get 
            {
                if (this.Id == (short)OvertimeTypes.Normal)
                {
                    return Labels.NormalDay;
                }
                else if (this.Id == (short)OvertimeTypes.Off)
                {
                    return Labels.OffDay;
                }
                else if (this.Id == (short)OvertimeTypes.Holiday)
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
