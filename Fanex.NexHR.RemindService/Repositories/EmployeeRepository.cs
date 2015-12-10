// --------------------------------------------------------------------------
// <copyright file="EmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.18">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.ReminderService.Models;

    public class EmployeeRepository : IEmployeeRepository
    {
        public IList<EmployeeMissingCheckInOutModel> GetEmployeeMissingCheckInOut(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var employees = db.Query<EmployeeMissingCheckInOutModel>(criteria).ToList();

                return employees;
            }
        }

        public IList<EmployeeAbsentModel> GetEmployeeAbsent(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var employees = db.Query<EmployeeAbsentModel>(criteria).ToList();

                return employees;
            }
        }

        public IList<EmployeeLateInEarlyOutModel> GetEmployeeLateInEarlyOut(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var employees = db.Query<EmployeeLateInEarlyOutModel>(criteria).ToList();

                return employees;
            }
        }

        public IList<EmployeeOffDayModel> GetEmployeeAboveOffDay(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var employees = db.Query<EmployeeOffDayModel>(criteria).ToList();

                return employees;
            }
        }
    }
}
