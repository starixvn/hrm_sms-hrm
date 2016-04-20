namespace Fanex.NexHR.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Repositories.Interfaces;

    public class DisciplineRepository : IDisciplineRepository
    {
        public IList<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryModel, Employee, Team, DisciplineSummaryModel>(
                    (discipline, employee, group) =>
                    {
                        discipline.Employee = employee;
                        discipline.Group = group;

                        return discipline;
                    },
                    "EmpId, GroupId",
                    criteria.GetSpParams()).ToList();
            }
        }
    }
}