// --------------------------------------------------------------------------
// <copyright file="IDepartmentRepository.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.21">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IDepartmentRepository
    {
        Department GetById(ICriteria criteria);

        IList<Department> GetDepartment(ICriteria criteria);
    }
}
