// --------------------------------------------------------------------------
// <copyright file="ShiftByStatusViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.1">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Shifts
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;

    /// <summary>
    /// ShiftByStatus ViewModel.
    /// </summary>
    public class ShiftByStatusViewModel
    {
        public int StatusId { get; set; }

        public IList<Shift> Shifts { get; set; }

        public IList<SelectListItem> Status
        {
            get
            {
                var status = new List<SelectListItem> 
                { 
                    new SelectListItem { Value = ((int)ShiftStatus.All).ToString(), Text = ShiftStatus.All.ToString() },
                    new SelectListItem { Value = ((int)ShiftStatus.Active).ToString(), Text = ShiftStatus.Active.ToString() },
                    new SelectListItem { Value = ((int)ShiftStatus.Disabled).ToString(), Text = ShiftStatus.Disabled.ToString() },
                };

                return status;
            }
        }
    }
}
