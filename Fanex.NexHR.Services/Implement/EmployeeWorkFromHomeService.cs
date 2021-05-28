namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Models;

    public class EmployeeWorkFromHomeService : IEmployeeWorkFromHomeService
    {
        private readonly IBaseService baseService;

        public EmployeeWorkFromHomeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<EmployeeWorkFromHome> GetAll(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetEmployeeWorkFromHomeCriteria 
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            return baseService.GetData<EmployeeWorkFromHome>(criteria).ToList();
        }

        public EmployeeWorkFromHome GetById(int id)
        {
            var criteria = new GetEmployeeWorkFromHomeByIdCriteria { EmployeeWorkFromHomeId = id };
            return baseService.GetDataEntity<EmployeeWorkFromHome>(criteria);
        }

        public CommandResult Insert(InsertEmployeeWorkFromHomeCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}