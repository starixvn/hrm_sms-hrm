namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Core.Models;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;

    public interface IEmployeeWorkFromHomeService
    {
        IList<EmployeeWorkFromHome> GetAll(DateTime fromDate, DateTime toDate);

        EmployeeWorkFromHome GetById(int id);
        CommandResult Insert(InsertEmployeeWorkFromHomeCriteria criteria);

        CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria);

        CommandResult Delete(int id);
    }
}