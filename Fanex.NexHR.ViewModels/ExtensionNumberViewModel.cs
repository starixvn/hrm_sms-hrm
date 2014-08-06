// --------------------------------------------------------------------------
// <copyright file="ExtensionNumberViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    /// <summary>
    /// Extension Number ViewModel.
    /// </summary>
    public class ExtensionNumberViewModel
    {
        public string Name { get; set; }

        public IList<ExtensionNoModel> ListExt { get; set; }
    }
}
