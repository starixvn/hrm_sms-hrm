namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;

    public class DashboardService : IDashboardService
    {
        private readonly IBaseService baseService;

        public DashboardService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public MySpaceModel GetMySpace(int employeeId, DateTime date)
        {
            var criteria = new GetMySpaceCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            return baseService.GetDataEntity<MySpaceModel>(criteria);
        }
    }
}