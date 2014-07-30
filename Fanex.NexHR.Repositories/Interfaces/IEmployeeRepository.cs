// --------------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeRepository
    {
        Employee GetEmployeeById(ICriteria criteria);

        Employee GetEmployeeByCode(ICriteria criteria);

        IList<Employee> GetEmployees(string query, string groupIds, bool? enable = null, bool? isDelete = null, int? titleId = null, bool? isLocal = null);

        IList<Employee> GetEmployeesByRole(ICriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees();

        IList<SupervisorNameModel> GetEmployeeCanClaimOvertime();
    }
}
