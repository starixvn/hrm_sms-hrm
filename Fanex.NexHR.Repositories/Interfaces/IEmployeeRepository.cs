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
        string NewEmployeeCode(ICriteria criteria);

        IList<SupervisorNameModel> GetListSupervisorName(ICriteria criteria);

        IList<SupervisorModel> GetEmployeeSupervisor(ICriteria criteria);

        Employee GetEmployeeById(ICriteria criteria);

        Employee GetEmployeeByCode(ICriteria criteria);

        UserLoginModel GetUserLoginByDomainUserName(ICriteria criteria);

        UserLoginModel GetUserLoginByCode(ICriteria criteria);

        UserLoginModel GetUserLoginByEmployeeId(ICriteria criteria);

        IList<Employee> GetEmployees(string query, string groupIds, bool? enable = null, bool? isDelete = null, int? titleId = null, bool? isLocal = null);

        IList<Employee> GetAll();

        IList<Country> GetAllCountry(ICriteria criteria);

        Country GetCountryByCode(ICriteria criteria);

        int Insert(CreateEmployeeViewModel model, out int errorCode, out string message);

        bool Delete(int id, DateTime resignedDate);

        int Update(Employee model, out int errorCode, out string message);

        bool UpdatePersonalInformation(Employee model, out int errorCode, out string message);

        EmployeeShift GetEmployeeShift(int id);

        bool UpdateEmployeeShift(EmployeeShift model);

        IList<WeekWorking> GetWeekWorking(int id, DateTime date);

        bool UpdateWeekWorking(EmployeeShift model, DateTime date);

        MySpaceViewModel GetMySpace(int id);

        IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> GetDateOfEmployeeForgotAndLateEarlyIO(int id);

        IList<DateTime> GetOffDateAndHolidayDate(int empId, DateTime fromDate, DateTime toDate);

        IList<HrLeaderDashboardModel> GetHrLeaderDashboard(DateTime date, int? employeeId = null);

        bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate);

        IList<Employee> GetMembersByManager(ICriteria criteria);

        IList<Employee> GetEmployeesByRole(ICriteria criteria);

        int UpdatePasswordResetStatus(int empId, bool passwordReset, out int errorCode, out string message);

        bool AdjustAnnualLeaves(
            string empIds,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy,
            out int errorCode,
            out string message);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans);

        bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id);

        bool UpdateAnnualLeaveTransaction(
            long id,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime modifiedDate,
            int modifiedBy);

        bool InsertAnnualLeaveTransaction(
            int empId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees();

        IList<UserChangedShift> GetUserChangedShift(DateTime date);

        IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null);

        int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate);

        bool ResetAnnualLeaves(string empValue, string description, DateTime fromDate, int resetDays, DateTime createdDate, int createdBy);

        int ChangePassword(int empId, string password, bool? passwordReset);

        bool UpdateSupervisor(SupervisorViewModel model, int modifiedBy);

        int InsertEmployeeShiftManager(ShiftManagerViewModel shiftManagerModel, int modifiedBy, out int errorCode, out string message);

        int UpdateOrderShiftArrange(MemberScheduleOrderModel memberOrder, out int errorCode, out string message);

        IList<SupervisorNameModel> GetEmployeeCanClaimOvertime();
    }
}
