// --------------------------------------------------------------------------
// <copyright file="ManpowerCategoryViewModel.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.4">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report;

    public class ManpowerCategoryViewModel
    {
        public int Year { get; set; }

        public IList<ManpowerCategoryModel> Manpowers { get; set; }
    }
}
