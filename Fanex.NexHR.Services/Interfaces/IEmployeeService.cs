namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public interface IEmployeeService
    {
        string GenerateEmployeeCode();

        IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria);

        IList<SupervisorModel> GetEmployeeSupervisorByRoleId(int employeeId, RoleSystem roleId);

        Employee GetEmployeeById(int id);

        IList<Employee> GetEmployees(GetAllEmployeeCriteria criteria);

        IList<Employee> GetMemberByManager(GetMemberByManagerCriteria criteria);

        IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria);

        IList<Country> GetAllCountry();

        EmployeeShift GetShiftByEmployeeId(int id);

        MySpaceModel GetMySpace(int employeeId, DateTime? date);

        IList<DateTime> GetOffDateAndHolidayDate(GetOffDateAndHolidayDateCriteria criteria);

        IList<DateTime> GetNotOffOrHolidayDate(GetOffDateAndHolidayDateCriteria criteria);

        IList<HRLeaderDashboardModel> GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveTransactionViewModel GetAnnualLeaveTransaction(int leaveTransId);

        IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(GetEmployeeHalfDayOffCriteria criteria);

        int GetDateRange(GetDateRangeCriteria criteria);

        IList<DateTime> GetUnavailableDates(GetUnavailableRequestDateCriteria criteria);

        IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria);

        IList<SupervisorNameModel> GetSupervisorByQuery(GetEmployeeSupervisorByQueryCriteria criteria);

        IEnumerable<Supervisor> GetDisciplineSettingByEmployeeId(int employeeId);

        IList<Employee> GetEmployeeByGender(string query, int gender);

        CommandResult EditEmployeeSupervisor(int actorId, int employeeId, short roleId, IEnumerable<EditSectionSupervisor> supervisors);
    }
}