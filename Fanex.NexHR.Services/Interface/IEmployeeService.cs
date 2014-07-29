// --------------------------------------------------------------------------
// <copyright file="IEmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeSupervisor;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeService
    {
        string NewEmployeeCode(GetNewEmployeeCodeCriteria criteria);

        IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria);

        IList<SupervisorModel> GetEmployeeSupervisor(GetEmployeeSupervisorCriteria criteria);

        UserLoginModel GetUserLoginByDomainUserName(GetUserLoginByDomainUserNameCriteria criteria);

        UserLoginModel GetUserLoginByCode(GetUserLoginByCodeCriteria criteria);

        UserLoginModel GetUserLoginByEmployeeId(GetEmployeeByIdCriteria criteria);

        Employee GetEmployeeById(GetEmployeeByIdCriteria criteria);

        Employee GetEmployeeByCode(GetEmployeeByCodeCriteria criteria);

        IList<Employee> GetEmployees(string query, string groupIds, bool? enable = null, bool? isDelete = null, int? titleId = null, bool? isLocal = null);

        IList<Employee> GetMembersByManager(GetEmployeeByManagerCriteria criteria);

        IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria);

        IList<Country> GetAllCountry(GetAllCountryCriteria criteria);

        Country GetCountryByCode(GetCountryByCodeCriteria criteria);

        CommandResult AdjustAnnualLeaves(AdjustEmployeeAnnualLeaveCriteria criteria);

        CommandResult ResetAnnualLeaves(ResetAnnualLeaveCriteria criteria);

        CommandResult Insert(InsertEmployeeCriteria criteria);

        CommandResult Delete(DeleteEmployeeCriteria criteria);

        CommandResult Update(UpdateEmployeeCriteria criteria);

        CommandResult UpdatePersonalInformation(UpdateEmployeePersonalInformationCriteria criteria);

        EmployeeShift GetEmployeeShift(GetEmployeeShiftCriteria criteria);

        bool UpdateEmployeeShift(EmployeeShift model);

        IList<WeekWorking> GetWeekWorking(int id, DateTime date);

        bool UpdateWeekWorking(EmployeeShift model, DateTime date);

        MySpaceViewModel GetMySpace(int id);

        IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> GetDateOfEmployeeForgotAndLateEarlyIO(int id);

        IList<DateTime> GetOffDateAndHolidayDate(int empId, DateTime fromDate, DateTime toDate);

        IList<DateTime> GetOffAndHoliday(int empId, DateTime fromDate, DateTime toDate);

        IList<HrLeaderDashboardModel> GetHrLeaderDashboard(DateTime date, int? employeeId = null);

        bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id);

        bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy);

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

        IList<UserChangedShift> GetUserChangedShift(DateTime date);

        IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null);

        int GetDateRange(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DateTime> GetUnavailableDates(int employeeId, DateTime fromDate, DateTime toDate);

        int ValidateUser(string code, string password);

        int ChangePassword(int empId, string oldPassword, string password);

        int ResetPassword(int empId, string originalPassword);

        bool UpdateSupervisor(SupervisorViewModel model, int modifiedBy);

        int InsertEmployeeShiftManager(ShiftManagerViewModel shiftManagerModel, int modifiedBy);

        int UpdateOrderShiftArrange(MemberScheduleOrderModel memberOrder, out int errorCode, out string message);

        int UpdatePasswordResetStatus(int empId, bool passwordReset);

        IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria);
    }
}
