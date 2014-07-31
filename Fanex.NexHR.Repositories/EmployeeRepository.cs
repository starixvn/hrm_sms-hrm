// --------------------------------------------------------------------------
// <copyright file="EmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.23">Created</change>
//   <change who="Anders.Le" date="2012.5.23">Modify Get function</change>
// </history>
namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Dapper;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee Repository.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        /// <summary>
        /// Method get Employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Employee entity.</returns>
        public Employee GetEmployeeById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameters: criteria.GetSpParams());

                return entity;
            }
        }

        /// <summary>
        /// Method get Employee by code.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Employee entity.</returns>
        public Employee GetEmployeeByCode(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Employee entity = db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameters: criteria.GetSpParams());

                return entity;
            }
        }

        /// <summary>
        /// Get All Employees.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List Of Employee.</returns>
        public IList<Employee> GetEmployees(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = db.Query<Employee, dynamic, Employee>(
                     (e, t) =>
                     {
                         e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                         return e;
                     },
                     splitOn: "TeamID",
                     parameterValues: criteria.GetSpParams());
                return entityList.ToList();
            }
        }

        /// <summary>
        /// Get Employees by role.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of employee.</returns>
        public IList<Employee> GetEmployeesByRole(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entity = db.Query<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameterValues: criteria.GetSpParams()).ToList();

                return entity;
            }
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees()
        {
            using (ObjectDb db = new ObjectDb("GetMissingCheckInOutList"))
            {
                return db.Query<MissingCheckInOutModel>().ToList();
            }
        }

        public IList<SupervisorNameModel> GetEmployeeCanClaimOvertime()
        {
            using (ObjectDb db = new ObjectDb("OverTime_GetListEmpClaimOT"))
            {
                return db.Query<SupervisorNameModel>().ToList();
            }
        }
    }
}
