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

        public IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> GetDateOfEmployeeForgotAndLateEarlyIO(int empId)
        {
            using (var db = new ObjectDb("Report_DateOfEmployeeForgotAndLateEarlyIO"))
            {
                var date = DateTime.Now.ToShortDateString();
                return db.Query<DateOfEmployeeForgotAndLateEarlyIOViewModel>(new { empId, date }).ToList();
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

        public AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int leaveTransId)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_GetByID"))
            {
                return db.QueryEntity<AnnualLeaveBalanceModel>(new { leaveTransId });
            }
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees()
        {
            using (ObjectDb db = new ObjectDb("GetMissingCheckInOutList"))
            {
                return db.Query<MissingCheckInOutModel>().ToList();
            }
        }

        public IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? empId = null, int? groupId = null)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetHalfDayOff"))
            {
                return db.Query<HalfDayOff>(new { reportDate, empId, groupId }).ToList();
            }
        }

        /// <summary>
        /// Method get Range date.
        /// </summary>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="fromDate">Range from date.</param>
        /// <param name="toDate">Range to date.</param>
        /// <returns>Number days.</returns>
        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            int numberDay = 0;
            if (toDate.Date >= fromDate.Date)
            {
                using (var db = new ObjectDb("EmployeeCalendar_GetRangeDate"))
                {
                    db.AddInParameter("@EmpId", DbType.Int32, employeeId);
                    db.AddInParameter("@fromDate", DbType.DateTime, fromDate);
                    db.AddInParameter("@toDate", DbType.DateTime, toDate);

                    numberDay = (int)db.ExecuteScalar();
                }
            }

            return numberDay;
        }

        public IEnumerable<DateTime> GetDisableRequestDate(int empId, DateTime fromDate, DateTime toDate)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetUnavailRequestDate"))
            {
                var unavailableDates = db.Query<DateTime>(new { empId = empId, fromDate = fromDate, toDate = toDate });
                return unavailableDates;
            }
        }

        /// <summary>
        /// Method change password.
        /// </summary>
        /// <param name="empId">Employee's id.</param>
        /// <param name="password">Current password.</param>
        /// <param name="passwordReset">Check password is reset or not.</param>
        /// <returns>An number to check status.</returns>
        public int ChangePassword(int empId, string password, bool? passwordReset)
        {
            using (var db = new ObjectDb("Employee_ChangePassword"))
            {
                db.AddInParameter("@empId", DbType.Int32, empId);
                db.AddInParameter("@password", DbType.String, password);
                db.AddInParameter("@PasswordReset", DbType.Boolean, passwordReset);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                int errorCode = (int)db.GetParameterValue("ErrCode");

                return errorCode;
            }
        }

        /// <summary>
        /// Insert info or update info to Employee Supervisor.
        /// </summary>
        /// <param name="model">Supervisor Model.</param>
        /// <param name="modifiedBy">Modify by.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateSupervisor(SupervisorViewModel model, int modifiedBy)
        {
            using (ObjectDb db = new ObjectDb("EmployeeSupervisorUpdate"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, model.EmployeeId);
                db.AddInParameter("@Role", DbType.Int16, model.SupervisorRole);
                db.AddInParameter("@EmpIdLeaves", DbType.String, model.RoleLeaves);
                db.AddInParameter("@EmpIdChangeShifts", DbType.String, model.RoleChangeShifts);
                db.AddInParameter("@EmpIdLateInEarlyOuts", DbType.String, model.RoleLateInEarlyOuts);
                db.AddInParameter("@EmpIdCheckInOuts", DbType.String, model.RoleCheckInOuts);
                db.AddInParameter("@EmpIdOvertimes", DbType.String, model.RoleOvertimes);
                db.AddInParameter("@EmpIdKeepInViews", DbType.String, model.RoleKeepInViews);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);

                return db.ExecuteNonQuery() != 0;
            }
        }

        public IList<SupervisorNameModel> GetEmployeeCanClaimOvertime()
        {
            using (ObjectDb db = new ObjectDb("OverTime_GetListEmpClaimOT"))
            {
                return db.Query<SupervisorNameModel>().ToList();
            }
        }

        /// <summary>
        /// Method Insert Employee Shift Manager.
        /// </summary>
        /// <param name="shiftManagerModel">Shift Manager Model.</param>
        /// <param name="modifiedBy">Modified by.</param>
        /// <param name="errorCode">Error code returned.</param>
        /// <param name="message">Error Message returned.</param>
        /// <returns>Error code.</returns>
        public int InsertEmployeeShiftManager(ShiftManagerViewModel shiftManagerModel, int modifiedBy, out int errorCode, out string message)
        {
            using (ObjectDb db = new ObjectDb("EmployeeSupervisor_InsertEmpMngShiftList"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, shiftManagerModel.EmployeeId);
                db.AddInParameter("@EmpIdShift", DbType.String, shiftManagerModel.RoleShiftArrange);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode;
            }
        }

        /// <summary>
        /// Method Order for Shift Manager.
        /// </summary>
        /// <param name="memberOrder">MemberScheduleOrder Model.</param>
        /// <param name="errorCode">Error code returned.</param>
        /// <param name="message">Error Message returned.</param>
        /// <returns>Error code.</returns>
        public int UpdateOrderShiftArrange(MemberScheduleOrderModel memberOrder, out int errorCode, out string message)
        {
            using (ObjectDb db = new ObjectDb("EmployeeSupervisor_UpdateOrderShiftArrange"))
            {
                db.AddInParameter("@SupervisorId", DbType.Int32, memberOrder.SupervisorId);
                db.AddInParameter("@EmployeeOrders", DbType.String, memberOrder.MemberOrderXML);
                db.AddInParameter("@ModifiedBy", DbType.Int32, memberOrder.ModifiedBy);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode;
            }
        }

        /// <summary>
        /// Update Status for Password reset.
        /// </summary>
        /// <param name="empId">Employee id.</param>
        /// <param name="passwordReset">Password reset status.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Error code to check.</returns>
        public int UpdatePasswordResetStatus(int empId, bool passwordReset, out int errorCode, out string message)
        {
            using (ObjectDb db = new ObjectDb("Employee_UpdatePasswordResetStatusByEmpId"))
            {
                db.AddInParameter("@empId", DbType.Int32, empId);
                db.AddInParameter("@PasswordReset", DbType.Boolean, passwordReset);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode;
            }
        }
    }
}
