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
        public string NewEmployeeCode(ICriteria criteria)
        {
            var code = string.Empty;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                code = db.QueryEntity<string>(criteria.GetSpParams());
            }

            return code;
        }

        /// <summary>
        /// Method get Employee.
        /// </summary>
        /// <param name="employeeId">The id of an employee.</param>
        /// <returns>Employee entity.</returns>
        public Employee GetEmployeeById(int? employeeId)
        {
            using (var db = new ObjectDb("GetEmployee"))
            {
                var entity = db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameters: new { Empid = employeeId });

                return entity;
            }
        }

        /// <summary>
        /// Method get Employee by code.
        /// </summary>
        /// <param name="code">Employee's code.</param>
        /// <returns>Employee entity.</returns>
        public Employee GetEmployeeByCode(string code)
        {
            using (var db = new ObjectDb("Employee_GetEmployeeByCode"))
            {
                Employee entity = db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameters: new { code = code });

                return entity;
            }
        }

        /// <summary>
        /// Get User By Domain User Name.
        /// </summary>
        /// <param name="domainUsername">Domain User Name.</param>
        /// <returns>UserLoginModel Entity.</returns>
        public UserLoginModel GetUserLoginByDomainUserName(string domainUsername = null)
        {
            using (var db = new ObjectDb("GetEmployeeByDomainUserName"))
            {
                var entity = db.QueryEntity<UserLoginModel>(new { domainUsername = domainUsername });

                return entity;
            }
        }

        /// <summary>
        /// Method get Employee by code.
        /// </summary>
        /// <param name="code">Employee's code.</param>
        /// <returns>Employee entity.</returns>
        public UserLoginModel GetUserLoginByCode(string code)
        {
            using (var db = new ObjectDb("Employee_GetEmployeeByCode"))
            {
                var entity = db.QueryEntity<UserLoginModel>(new { code = code });

                return entity;
            }
        }

        /// <summary>
        /// Method get Employee by id.
        /// </summary>
        /// <param name="employeeId">Employee's id.</param>
        /// <returns>Employee entity.</returns>
        public UserLoginModel GetUserLoginByEmployeeId(int employeeId)
        {
            using (var db = new ObjectDb("GetEmployee"))
            {
                var entity = db.QueryEntity<UserLoginModel>(new { Empid = employeeId });

                return entity;
            }
        }

        /// <summary>
        /// Method get all Employees.
        /// </summary>
        /// <returns>All employee.</returns>
        public IList<Employee> GetAll()
        {
            using (var db = new ObjectDb("GetEmployees"))
            {
                var p = new DynamicParameters();
                p.Add("@query", null);
                p.Add("@pageNumber", 1);
                p.Add("@pageSize", int.MaxValue);
                p.Add("@totalRow", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var entityList = db.Query<Employee, dynamic, Employee>(
                       (e, t) =>
                       {
                           e.Team = new Team { Id = t.TeamID, Code = t.Team, Name = t.TeamName };
                           return e;
                       },
                       splitOn: "TeamID",
                       parameterValues: p);

                return entityList.ToList();
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

        public IList<Employee> GetEmployeesForRelatedTeam(int employeeId)
        {
            using (var db = new ObjectDb("Employee_GetEmployeeForRelatedTeam"))
            {
                var entityList = db.Query<Employee>(new { empId = employeeId }).ToList();

                return entityList;
            }
        }

        /// <summary>
        /// Method insert an Employee.
        /// </summary>
        /// <param name="model">Employee View Model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Error Code.</returns>
        public int Insert(CreateEmployeeViewModel model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("Employee_Information_Insert"))
            {
                db.AddInParameter("@fullName", DbType.String, model.FullName);
                db.AddInParameter("@nickName", DbType.String, model.NickName);
                db.AddInParameter("@domainUsername", DbType.String, model.DomainUsername);
                db.AddInParameter("@code", DbType.String, model.Code);
                db.AddInParameter("@monShift", DbType.String, model.MonShift);
                db.AddInParameter("@tueShift", DbType.String, model.TueShift);
                db.AddInParameter("@wedShift", DbType.String, model.WedShift);
                db.AddInParameter("@thuShift", DbType.String, model.ThuShift);
                db.AddInParameter("@friShift", DbType.String, model.FriShift);
                db.AddInParameter("@satShift", DbType.String, model.SatShift);
                db.AddInParameter("@sunShift", DbType.String, model.SunShift);
                db.AddInParameter("@mainGroupId", DbType.Int16, model.TeamId);
                db.AddInParameter("@createdDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@createBy", DbType.Int16, model.ProcessBy);
                db.AddInParameter("@password", DbType.String, model.Password);
                db.AddInParameter("@IsDefaultSchedule", DbType.Boolean, model.IsDefaultSchedule);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;
                return errorCode;
            }
        }

        public bool Delete(int id, DateTime resignedDate)
        {
            using (var db = new ObjectDb("Employee_Delete"))
            {
                db.AddInParameter("@Empid", DbType.Int32, id);
                db.AddInParameter("@ResignedDate", DbType.DateTime, resignedDate);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Method update Employee.
        /// </summary>
        /// <param name="model">Employee model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Value of Error code.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public int Update(Employee model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("Employee_UpdateEmployee"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, model.Id);
                db.AddInParameter("@Code", DbType.String, model.Code.Trim());
                db.AddInParameter("@DomainUsername", DbType.String, model.DomainUsername);
                db.AddInParameter("@FullName", DbType.String, model.FullName);
                db.AddInParameter("@NickName", DbType.String, model.NickName);
                db.AddInParameter("@DOB", DbType.DateTime, model.DOB);
                db.AddInParameter("@Email", DbType.String, model.Email);
                db.AddInParameter("@JobTitleId", DbType.Int32, model.JobTitleId);
                db.AddInParameter("@Enabled", DbType.Boolean, model.Enabled);
                db.AddInParameter("@Phone", DbType.String, model.Phone);
                db.AddInParameter("@Gender", DbType.Int32, model.Gender);
                db.AddInParameter("@Roles", DbType.String, model.Roles);
                db.AddInParameter("@JoinedDate", DbType.DateTime, model.JoinedDate);
                db.AddInParameter("@CompanyDate", DbType.DateTime, model.CompanyDate);
                db.AddInParameter("@ConfirmedDate", DbType.DateTime, model.ConfirmedDate);
                db.AddInParameter("@ResignedDate", DbType.DateTime, model.ResignedDate);
                db.AddInParameter("@Skype", DbType.String, model.Skype);
                db.AddInParameter("@Yahoo", DbType.String, model.Yahoo);
                db.AddInParameter("@Marriage", DbType.Byte, model.Marriage);
                db.AddInParameter("@PermanentAddress", DbType.String, model.Address);
                db.AddInParameter("@CurrentAddress", DbType.String, model.CurrentAddress);
                db.AddInParameter("@IdCardNumber", DbType.String, model.CardNo);
                db.AddInParameter("@PassportNo", DbType.String, model.PassportNo);
                db.AddInParameter("@PassportExp", DbType.DateTime, model.PassportExp);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@ModifiedBy", DbType.Int32, model.Modifier.Id);
                db.AddInParameter("@Password", DbType.String, model.Password);
                db.AddInParameter("@GroupId", DbType.Int32, model.Team.Id);
                db.AddInParameter("@IsLocal", DbType.Boolean, model.IsLocal);
                db.AddInParameter("@CountryCode", DbType.String, model.CountryCode);
                db.AddInParameter("@CanOT", DbType.Boolean, model.CanOT);
                db.AddInParameter("@IsDefaultSchedule", DbType.Boolean, model.IsDefaultSchedule);
                db.AddInParameter("@WorkingDays", DbType.Int32, model.WorkingDays);
                db.AddInParameter("@WorkingHours", DbType.Int32, model.WorkingHours);
                db.AddInParameter("@IsTopManager", DbType.Boolean, model.IsTopManager);
                db.AddInParameter("@OffDaysbyWeek", DbType.Int32, model.OffDaysbyWeek);
                db.AddInParameter("@Unit", DbType.String, model.Unit);
                db.AddInParameter("@PlaceOfbirth", DbType.String, model.PlaceOfBirth);
                db.AddInParameter("@Ethnic", DbType.String, model.Ethnic);
                db.AddInParameter("@Native", DbType.String, model.Native);
                db.AddInParameter("@Religion", DbType.String, model.Religion);
                db.AddInParameter("@DateOfIDCard", DbType.DateTime, model.DateOfIdCard);
                db.AddInParameter("@PlaceOfIDCard", DbType.String, model.PlaceOfIdCard);
                db.AddInParameter("@HomePhone", DbType.String, model.HomePhone);
                db.AddInParameter("@Qualification", DbType.String, model.Qualification);
                db.AddInParameter("@Major", DbType.String, model.Major);
                db.AddInParameter("@BankAccount", DbType.String, model.BankAccount);
                db.AddInParameter("@TaxCode", DbType.String, model.TaxCode);
                db.AddInParameter("@SocialInsuranceNo", DbType.String, model.SocialInsuranceNo);
                db.AddInParameter("@RegisteredHospital", DbType.String, model.RegisteredHospital);
                db.AddInParameter("@IsExpatUseHoliday", DbType.Boolean, model.IsExpatUseHoliday);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);
                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode;
            }
        }

        /// <summary>
        /// Method update Employee.
        /// </summary>
        /// <param name="model">Employee model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>
        /// Value of Error code.
        /// </returns>
        public bool UpdatePersonalInformation(Employee model, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("Employee_Information_UpdateByEmpId"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, model.Id);
                db.AddInParameter("@DOB", DbType.DateTime, model.DOB);
                db.AddInParameter("@PlaceOfBirth", DbType.String, model.PlaceOfBirth);
                db.AddInParameter("@Ethnic", DbType.String, model.Ethnic);
                db.AddInParameter("@Native", DbType.String, model.Native);
                db.AddInParameter("@Religion", DbType.String, model.Religion);
                db.AddInParameter("@DateOfIdCard", DbType.DateTime, model.DateOfIdCard);
                db.AddInParameter("@PlaceOfIdCard", DbType.String, model.PlaceOfIdCard);
                db.AddInParameter("@HomePhone", DbType.String, model.HomePhone);
                db.AddInParameter("@Qualification", DbType.String, model.Qualification);
                db.AddInParameter("@Major", DbType.String, model.Major);
                db.AddInParameter("@Phone", DbType.String, model.Phone);
                db.AddInParameter("@Gender", DbType.Byte, model.Gender);
                db.AddInParameter("@Skype", DbType.String, model.Skype);
                db.AddInParameter("@Yahoo", DbType.String, model.Yahoo);
                db.AddInParameter("@ChineseName", DbType.String, model.ChineseName);
                db.AddInParameter("@Marriage", DbType.Byte, model.Marriage);
                db.AddInParameter("@PermanentAddress", DbType.String, model.Address);
                db.AddInParameter("@CurrentAddress", DbType.String, model.CurrentAddress);
                db.AddInParameter("@IdCardNumber", DbType.String, model.CardNo);
                db.AddInParameter("@PassportNo", DbType.String, model.PassportNo);
                db.AddInParameter("@PassportExp", DbType.DateTime, model.PassportExp);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@ModifiedBy", DbType.Int32, model.Modifier.Id);
                db.AddInParameter("@CountryCode", DbType.String, model.CountryCode);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);
                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                return errorCode == 0 ? true : false;
            }
        }

        public EmployeeShift GetEmployeeShift(int empID)
        {
            using (var db = new ObjectDb("EmployeeShift_Get"))
            {
                return db.QueryEntity<EmployeeShift>(new { empID });
            }
        }

        /// <summary>
        /// Method update Employee's shift.
        /// </summary>
        /// <param name="model">EmployeeShift model.</param>
        /// <returns>Boolean to check updated status.</returns>
        public bool UpdateEmployeeShift(EmployeeShift model)
        {
            using (var db = new ObjectDb("EmployeeShift_Update"))
            {
                db.AddInParameter("@Empid", DbType.Int32, model.EmpID);
                db.AddInParameter("@monShift", DbType.Int32, model.MonShift);
                db.AddInParameter("@tueShift", DbType.Int32, model.TueShift);
                db.AddInParameter("@wedShift", DbType.Int32, model.WedShift);
                db.AddInParameter("@thuShift", DbType.Int32, model.ThuShift);
                db.AddInParameter("@friShift", DbType.Int32, model.FriShift);
                db.AddInParameter("@satShift", DbType.Int32, model.SatShift);
                db.AddInParameter("@sunShift", DbType.Int32, model.SunShift);

                return db.ExecuteNonQuery() != 0;
            }
        }

        public IList<WeekWorking> GetWeekWorking(int id, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetWeekWorking"))
            {
                return db.Query<WeekWorking>(new { id, date }).ToList();
            }
        }

        /// <summary>
        /// Method update week working.
        /// </summary>
        /// <param name="model">EmployeeShift model.</param>
        /// <param name="date">Date time for updating.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateWeekWorking(EmployeeShift model, DateTime date)
        {
            using (var db = new ObjectDb("EmployeeCalendar_UpdateWeekWorking"))
            {
                db.AddInParameter("@id", DbType.Int32, model.EmpID);
                db.AddInParameter("@date", DbType.DateTime, date);
                db.AddInParameter("@monShift", DbType.Int32, model.MonShift);
                db.AddInParameter("@tueShift", DbType.Int32, model.TueShift);
                db.AddInParameter("@wedShift", DbType.Int32, model.WedShift);
                db.AddInParameter("@thuShift", DbType.Int32, model.ThuShift);
                db.AddInParameter("@friShift", DbType.Int32, model.FriShift);
                db.AddInParameter("@satShift", DbType.Int32, model.SatShift);
                db.AddInParameter("@sunShift", DbType.Int32, model.SunShift);
                return db.ExecuteNonQuery() != 0;
            }
        }

        public MySpaceViewModel GetMySpace(int empid)
        {
            using (var db = new ObjectDb("Report_EmployeeDashBoard"))
            {
                var date = DateTime.Now.ToShortDateString();
                return db.QueryEntity<MySpaceViewModel>(new { empid, date });
            }
        }

        public IList<HrLeaderDashboardModel> GetHrLeaderDashboard(DateTime date, int? empId = null)
        {
            using (var db = new ObjectDb("Report_HRLeaderDashBoard"))
            {
                return db.Query<HrLeaderDashboardModel>(new { empId, date }).ToList();
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

        public IList<DateTime> GetOffDateAndHolidayDate(int empId, DateTime fromDate, DateTime toDate)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetOffDate_HolidayDate"))
            {
                return db.Query<DateTime>(new { empId, fromDate, toDate }).ToList();
            }
        }

        /// <summary>
        /// Method update shift.
        /// </summary>
        /// <param name="id">Id of the Shift.</param>
        /// <param name="date">Date time to update.</param>
        /// <param name="shift">Shift number.</param>
        /// <param name="modifiedBy">Modified by.</param>
        /// <param name="modifiedDate">Modified date.</param>
        /// <returns>Boolean to check status.</returns>
        public bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate)
        {
            using (ObjectDb db = new ObjectDb("EmployeeCalendar_Update"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, id);
                db.AddInParameter("@date", DbType.DateTime, date);
                db.AddInParameter("@ShiftID", DbType.Int32, shift);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);

                return db.ExecuteNonQuery() != 0;
            }
        }

        public IList<Employee> GetMembersByManager(int managerId, short role, string keyword, int? groupId)
        {
            int excludedManager = 0;
            using (ObjectDb db = new ObjectDb("Employee_GetMembesByManager"))
            {
                var entity = db.Query<Employee>(new { managerId, role, excludedManager, groupId, keyword }).ToList();
                return entity;
            }
        }

        /// <summary>
        /// Get Employee By Role.
        /// </summary>
        /// <param name="roleIds">Role identify.</param>
        /// <param name="query">Query to database.</param>
        /// <param name="excludeIds">Exclude ids.</param>
        /// <returns>List Employee.</returns>
        public IList<Employee> GetEmployeesByRole(string roleIds, string query, string excludeIds)
        {
            using (var db = new ObjectDb("Employee_GetByRole"))
            {
                var entity = db.Query<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamID",
                        parameterValues: new { roleIds = roleIds, query = query, excludeIds = excludeIds }).ToList();

                return entity;
            }
        }

        /// <summary>
        /// Method adjust annual leave.
        /// </summary>
        /// <param name="empIds">Employees id.</param>
        /// <param name="description">Reason to adjust.</param>
        /// <param name="fromDate">From Date.</param>
        /// <param name="deductDays">Deduct days.</param>
        /// <param name="creditDays">Credit days.</param>
        /// <param name="createdDate">Created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <param name="errorCode">Error code returned.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool AdjustAnnualLeaves(
            string empIds,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy,
            out int errorCode,
            out string message)
        {
            var checkDescription = description != null ? description : string.Empty;
            using (ObjectDb db = new ObjectDb("AnnualLeave_InsertMultipleTransactions"))
            {
                db.AddInParameter("@EmpIds", DbType.String, empIds);
                db.AddInParameter("@Description", DbType.String, checkDescription);
                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@DeductDays", DbType.Decimal, deductDays);
                db.AddInParameter("@CreditDays", DbType.Decimal, creditDays);
                db.AddInParameter("@CreatedDate", DbType.DateTime, createdDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, createdBy);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);
                db.ExecuteScalar();
                errorCode = (int)db.GetParameterValue("ErrCode");
                message = db.GetParameterValue("ErrMsg") as string;

                if (errorCode == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool ResetAnnualLeaves(string empValue, string description, DateTime fromDate, int resetDays, DateTime createdDate, int createdBy)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_ResetMultipleTransactions"))
            {
                return db.ExecuteNonQuery(new { empValue, description, fromDate, resetDays, createdDate, createdBy }) != 0;
            }
        }

        public IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_GetTransByEmp"))
            {
                return db.Query<AnnualLeaveBalanceModel>(new { empId = id, year, excludedRequestTrans }).ToList();
            }
        }

        public bool DeleteAnnualLeaveTransaction(long leaveTransId, DateTime modifiedDate, int modifiedBy)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_DeleteTransaction"))
            {
                return db.ExecuteNonQuery(new { leaveTransId, modifiedDate, modifiedBy }) != 0;
            }
        }

        public AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int leaveTransId)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_GetByID"))
            {
                return db.QueryEntity<AnnualLeaveBalanceModel>(new { leaveTransId });
            }
        }

        /// <summary>
        /// Method update Annual Leave.
        /// </summary>
        /// <param name="leaveTransId">Leave Transaction id.</param>
        /// <param name="description">Leave's description.</param>
        /// <param name="fromDate">Affect from date.</param>
        /// <param name="deductDays">Deduct days.</param>
        /// <param name="creditDays">Credit days.</param>
        /// <param name="modifiedDate">Modified date.</param>
        /// <param name="modifiedBy">Modified by.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateAnnualLeaveTransaction(
            long leaveTransId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime modifiedDate,
            int modifiedBy)
        {
            var checkDescription = string.Empty;
            if (description == null)
            {
                description = checkDescription;
            }

            using (ObjectDb db = new ObjectDb("AnnualLeave_UpdateTransaction"))
            {
                return db.ExecuteNonQuery(new { leaveTransId, description, fromDate, deductDays, creditDays, modifiedDate, modifiedBy }) != 0;
            }
        }

        /// <summary>
        /// Method insert annual leave.
        /// </summary>
        /// <param name="empId">Employee's id.</param>
        /// <param name="description">Description to leave.</param>
        /// <param name="fromDate">Leave from date.</param>
        /// <param name="deductDays">Deduct days.</param>
        /// <param name="creditDays">Credit days.</param>
        /// <param name="createdDate">Created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <returns>Boolean to check status.</returns>
        public bool InsertAnnualLeaveTransaction(
            int empId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy)
        {
            using (ObjectDb db = new ObjectDb("AnnualLeave_InsertTransaction"))
            {
                return db.ExecuteNonQuery(new { empId, description, fromDate, deductDays, creditDays, createdDate, createdBy }) != 0;
            }
        }

        public IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees()
        {
            using (ObjectDb db = new ObjectDb("GetMissingCheckInOutList"))
            {
                return db.Query<MissingCheckInOutModel>().ToList();
            }
        }

        public IList<UserChangedShift> GetUserChangedShift(DateTime reportDate)
        {
            using (var db = new ObjectDb("EmployeeCalendar_GetChangedShift"))
            {
                return db.Query<UserChangedShift>(new { reportDate }).ToList();
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

        public IList<Country> GetAllCountry()
        {
            using (var db = new ObjectDb("Country_GetAll"))
            {
                return db.Query<Country>().ToList();
            }
        }

        public Country GetCountryByCode(string countryCode)
        {
            using (var db = new ObjectDb("Country_GetByCode"))
            {
                return db.QueryEntity<Country>(new { CountryCode = countryCode });
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

        public IList<SupervisorModel> GetEmployeeSupervisor(int empId, int role)
        {
            using (ObjectDb db = new ObjectDb("EmployeeSupervisor_GetRequestByEmpID"))
            {
                return db.Query<SupervisorModel>(new { EmpId = empId, Role = role }).ToList();
            }
        }

        public IList<SupervisorNameModel> GetListSupervisorName(int empId, int role, int? requestType)
        {
            using (ObjectDb db = new ObjectDb("EmployeeSupervisor_GetListSupervisorByEmpID"))
            {
                return db.Query<SupervisorNameModel>(new { EmpId = empId, Role = role, RequestType = requestType }).ToList();
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
