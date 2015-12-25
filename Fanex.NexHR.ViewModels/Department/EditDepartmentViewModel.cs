// --------------------------------------------------------------------------
// <copyright file="EditDepartmentViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.22">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Department
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class EditDepartmentViewModel
    {
        [Required]
        public int DeprtamentId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ManagerId { get; set; }

        public string Manager { get; set; }

        public int ParentId { get; set; }

        public bool IsActive { get; set; }

        public IList<Department> ParentDepartments { get; set; }
    }
}
