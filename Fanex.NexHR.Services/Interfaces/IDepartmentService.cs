// --------------------------------------------------------------------------
// <copyright file="IDepartmentService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.12.21">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;

    public interface IDepartmentService
    {
        Department GetById(GetDepartmentByIdCriteria criteria);

        IList<Department> GetDepartment(GetDepartmentByParentIdCriteria criteria);

        IList<Department> GetParentDepartment(GetParentByDepartmentCriteria criteria);

        CommandResult Add(InsertDepartmentCriteria criteria);

        CommandResult Edit(UpdateDepartmentCriteria criteria);

        CommandResult Delete(DeleteDepartmentCriteria criteria);
    }
}