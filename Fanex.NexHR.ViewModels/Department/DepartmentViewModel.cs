// --------------------------------------------------------------------------
// <copyright file="DepartmentViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.22">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Department
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class DepartmentViewModel
    {
        public bool? IsActive { get; set; }

        public IList<Department> Departments { get; set; }

        public int ParentId { get; set; }
    }
}
