// --------------------------------------------------------------------------
// <copyright file="AddDepartmentViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.22">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Department
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;

    public class AddDepartmentViewModel
    {
        public AddDepartmentViewModel()
        {
            IsActive = true;
        }

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
