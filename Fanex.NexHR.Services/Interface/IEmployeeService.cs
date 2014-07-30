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
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.DashBoard;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeSupervisor;
    using Fanex.NexHR.Criteria.MySpace;
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

        CommandResult UpdateEmployeeShift(UpdateEmployeeShiftCriteria criteria);

        MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria);

        IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> GetDateOfEmployeeForgotAndLateEarlyIO(GetEmployeeMissingRequestDateCriteria criteria);

        IList<DateTime> GetOffDateAndHolidayDate(GetOffDateAndHolidayDateCriteria criteria);

        IList<HrLeaderDashboardModel> GetHrLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(GetAnnualLeaveTransactionByIdCriteria criteria);

        CommandResult DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria);

        CommandResult UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria);

        CommandResult InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria);

        IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(GetEmployeeHalfDayOffCriteria criteria);

        int GetDateRange(GetDateRangeCriteria criteria);

        IList<DateTime> GetUnavailableDates(GetUnavailableRequestDateCriteria criteria);

        int ValidateUser(string code, string password);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult ResetPassword(int empId, string originalPassword);

        CommandResult UpdateSupervisor(UpdateSupervisorCriteria criteria);

        CommandResult InsertEmployeeShiftManager(InsertShiftManagerCriteria criteria);

        CommandResult UpdateOrderShiftArrange(UpdateOrderShiftArrangeCriteria criteria);

        CommandResult UpdatePasswordResetStatus(UpdatePasswordResetStatusCriteria criteria);

        IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria);
    }
}
