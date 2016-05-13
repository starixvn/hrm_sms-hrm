namespace Fanex.NexHR.Criteria.Discipline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Discipline;

    public class UpdateIgnoredDisciplineCriteria : CriteriaBase
    {
        private const string DisciplineTemp = "<Discipline WorkDate='{0}' EmployeeId='{1}' DisciplineTypeId='{2}' Ignored='{3}'></Discipline>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int ModifiedBy { get; set; }

        public string IgnoredDetails
        {
            get
            {
                return ConvertIgnoredDisciplineToXml();
            }
        }

        public int RoleId { get; set; }

        [SpParam(Ignored = true)]
        public IList<IgnoredDisciplineModel> IgnoredDisciplines { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeDiscipline_UpdateIgnored";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(IgnoredDetails);
        }

        private string ConvertIgnoredDisciplineToXml()
        {
            var violationXmlBuilder = new StringBuilder();

            if (IgnoredDisciplines != null && IgnoredDisciplines.Any())
            {
                violationXmlBuilder.Append(BeginRootTag);

                foreach (IgnoredDisciplineModel item in IgnoredDisciplines)
                {
                    violationXmlBuilder.AppendFormat(DisciplineTemp, item.WorkDate, item.EmployeeId, item.DisciplineTypeId, item.Ignored);
                }

                violationXmlBuilder.Append(EndRootTag);
            }

            return violationXmlBuilder.ToString();
        }
    }
}