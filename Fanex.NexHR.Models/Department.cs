// --------------------------------------------------------------------------
// <copyright file="Department.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.21">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    public class Department : EntityBase<int>
    {
        public Employee Manager { get; set; }

        public Department Parent { get; set; }

        public bool IsActive { get; set; }
    }
}
