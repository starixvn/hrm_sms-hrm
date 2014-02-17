// -----------------------------------------------------------------------
// <copyright file="ExtensionNumberViewModel.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
