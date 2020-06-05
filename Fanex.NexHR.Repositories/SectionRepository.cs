namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;

    public class SectionRepository : ISectionRepository
    {
        public Section GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var section = db.QueryEntity<Section, NewDepartment, Section>(
                        (s, d) =>
                        {
                            s.Department = d;
                            return s;
                        },
                        splitOn: "DepartmentId",
                        parameterSet: criteria);

                return section;
            }
        }

        public IList<Section> GetSection(GetSectionByParentIdCriteria criteria)
        {
            throw new System.NotImplementedException();
        }
    }
}