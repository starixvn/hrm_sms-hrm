// --------------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.18">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.ReminderService.Models;

    public interface IEmployeeRepository
    {
        IList<EmployeeMissingCheckInOutModel> GetEmployeeMissingCheckInOut(ICriteria criteria);

        IList<EmployeeAbsentModel> GetEmployeeAbsent(ICriteria criteria);

        IList<EmployeeLateInEarlyOutModel> GetEmployeeLateInEarlyOut(ICriteria criteria);

        IList<EmployeeOffDayModel> GetEmployeeAboveOffDay(ICriteria criteria);
    }
}
