namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Discipline;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ApproveRequestCriteria : CriteriaBase
    {
        private const string DisciplineTemp = "<Discipline WorkDate='{0}' EmployeeId='{1}' DisciplineTypeId='{2}' IsIgnored='{3}' IsConfirmed='{4}'></Discipline>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public long RequestId { get; set; }

        public int ProcessedBy { get; set; }

        public int RoleId { get; set; }

        public string Comment { get; set; }

        [SpParam(Ignored = true)]
        public string Action { get; set; }

        public string IgnoredDetails
        {
            get
            {
                return ConvertDisciplineItemToXml();
            }
        }

        [SpParam(Ignored = true)]
        public IList<IgnoredDisciplineModel> IgnoredDisciplines { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_ApproveRequest";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }

        private string ConvertDisciplineItemToXml()
        {
            var disciplineXmlBuilder = new StringBuilder();

            if (IgnoredDisciplines != null && IgnoredDisciplines.Any())
            {
                disciplineXmlBuilder.Append(BeginRootTag);

                foreach (IgnoredDisciplineModel item in IgnoredDisciplines)
                {
                    disciplineXmlBuilder.AppendFormat(DisciplineTemp, item.WorkDate, item.EmployeeId, item.DisciplineTypeId, item.IsIgnored, item.IsConfirmed);
                }

                disciplineXmlBuilder.Append(EndRootTag);
            }

            return disciplineXmlBuilder.ToString();
        }
    }
}