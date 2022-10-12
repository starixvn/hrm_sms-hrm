namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Dashboard;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Dashboard;

    public class DashboardService : IDashboardService
    {
        private readonly IBaseService baseService;

        public DashboardService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<SummaryRequest> GetMySpace(int employeeId, DateTime date)
        {
            var criteria = new GetMySpaceCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            return baseService.GetData<SummaryRequest>(criteria);
        }

        public IEnumerable<MemberRequest> GetMemberRequest(int employeeId, DateTime date)
        {
            var criteria = new GetMemberRequestCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            return baseService.GetData<MemberRequest>(criteria);
        }

        public IEnumerable<EmployeeLogin> GetCurrentEmployeeLogin()
        {
            var criteria = new GetCurrentEmployeeLoginCriteria();
            return baseService.GetData<EmployeeLogin>(criteria);
        }
    }
}