namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public interface IEmployeeService
    {
        string NewEmployeeCode(GetNewEmployeeCodeCriteria criteria);

        IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria);

        IList<SupervisorModel> GetEmployeeSupervisor(GetEmployeeSupervisorCriteria criteria);

        Employee GetEmployeeById(GetEmployeeByIdCriteria criteria);

        Employee GetEmployeeByCode(GetEmployeeByCodeCriteria criteria);

        IList<Employee> GetEmployees(GetAllEmployeeCriteria criteria);

        IList<Employee> GetMemberByManager(GetMemberByManagerCriteria criteria);

        IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria);

        IList<Country> GetAllCountry(GetAllCountryCriteria criteria);

        Country GetCountryByCode(GetCountryByCodeCriteria criteria);

        EmployeeShift GetEmployeeShift(GetEmployeeShiftCriteria criteria);

        MySpaceModel GetMySpace(GetMySpaceCriteria criteria);

        IList<DateTime> GetOffDateAndHolidayDate(GetOffDateAndHolidayDateCriteria criteria);

        IList<DateTime> GetNotOffOrHolidayDate(GetOffDateAndHolidayDateCriteria criteria);

        IList<HRLeaderDashboardModel> GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveTransactionViewModel GetAnnualLeaveTransaction(GetAnnualLeaveTransactionByIdCriteria criteria);

        IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(GetEmployeeHalfDayOffCriteria criteria);

        int GetDateRange(GetDateRangeCriteria criteria);

        IList<DateTime> GetUnavailableDates(GetUnavailableRequestDateCriteria criteria);

        IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria);

        IList<SupervisorNameModel> GetSupervisorByQuery(GetEmployeeSupervisorByQueryCriteria criteria);

        IList<SupervisorModel> GetDisciplineSettingByEmployeeId(GetDisciplineSettingByEmployeeId criteria);

        IList<Employee> GetEmployeeByGender(string query, int gender);
    }
}