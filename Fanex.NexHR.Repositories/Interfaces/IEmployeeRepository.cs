// --------------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IEmployeeRepository
    {
        Employee GetEmployeeById(ICriteria criteria);

        Employee GetEmployeeByCode(ICriteria criteria);

        IList<Employee> GetEmployees(ICriteria criteria);

        IList<Employee> GetEmployeesByRole(ICriteria criteria);
    }
}
