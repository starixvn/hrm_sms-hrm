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

        IList<DateOfEmployeeForgotAndLateEarlyIOViewModel> GetDateOfEmployeeForgotAndLateEarlyIO(int id);

        IList<Employee> GetEmployeesByRole(ICriteria criteria);

        int UpdatePasswordResetStatus(int empId, bool passwordReset, out int errorCode, out string message);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees();

        IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate);

        int ChangePassword(int empId, string password, bool? passwordReset);

        bool UpdateSupervisor(SupervisorViewModel model, int modifiedBy);

        int InsertEmployeeShiftManager(ShiftManagerViewModel shiftManagerModel, int modifiedBy, out int errorCode, out string message);

        int UpdateOrderShiftArrange(MemberScheduleOrderModel memberOrder, out int errorCode, out string message);

        IList<SupervisorNameModel> GetEmployeeCanClaimOvertime();
    }
}
