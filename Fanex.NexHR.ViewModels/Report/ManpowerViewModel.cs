// --------------------------------------------------------------------------
// <copyright file="ManpowerViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.4">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report;

    public class ManpowerViewModel
    {
        public int Year { get; set; }

        public IList<ManpowerModel> Manpowers { get; set; }
    }
}
