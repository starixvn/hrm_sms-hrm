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
        /// Query Employee.
        /// </summary>
        /// <param name="query">String query.</param>
        /// <param name="groupIds">The group's id.</param>
        /// <param name="enable">Employee's status.</param>
        /// <param name="isDelete">Check available.</param>
        /// <param name="titleId">Title's id.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>A List of Employee entity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<Employee> GetEmployees(
            string query,
            string groupIds,
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null,
            bool? isLocal = null)
        {
            using (var db = new ObjectDb("GetEmployees"))
            {
                var p = new DynamicParameters();
                int? disableUsers, deleteUsers;
                byte? localUser;
                if (enable == true)
                {
                    disableUsers = 0;
                }
                else if (enable == false)
                {
                    disableUsers = 1;
                }
                else
                {
                    disableUsers = null;
                }

                if (isDelete == true)
                {
                    deleteUsers = 1;
                }
                else if (isDelete == false)
                {
                    deleteUsers = 0;
                }
                else
                {
                    deleteUsers = null;
                }

                if (isLocal == false)
                {
                    localUser = 0;
                }
                else if (isLocal == true)
                {
                    localUser = 1;
                }
                else
                {
                    localUser = null;
                }

                p.Add("@titleId", titleId);
                var entityList = db.Query<Employee, dynamic, Employee>(
                     (e, t) =>
                     {
                         e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                         return e;
                     },
                     splitOn: "TeamID",
                     parameterValues: new 
                     { 
                         @query = query,
                         @groupIds = groupIds,
                         @totalRow = 0,
                         @enable = disableUsers,
                         @isDelete = deleteUsers,
                         @titleId = titleId,
                         @isLocal = localUser 
                     });
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
