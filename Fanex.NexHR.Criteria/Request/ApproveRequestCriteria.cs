namespace Fanex.NexHR.Criteria.Request
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Data.Repository;
    using Fanex.NexHR.Core.Constants;
    using Models.Discipline;

    public class ApproveRequestCriteria : CriteriaBase
    {
        private const string DisciplineTemp = "<Discipline WorkDate='{0}' EmployeeId='{1}' DisciplineTypeId='{2}' IsIgnored='{3}' IsConfirmed='{4}'></Discipline>";

        public long RequestId { get; set; }

        public int ProcessedBy { get; set; }

        public int RoleId { get; set; }

        public string Comment { get; set; }

        [SpParam(Ignored = true)]
        public string Action { get; set; }

        public string IgnoredDetails => ConvertDisciplineItemToXml();

        [SpParam(Ignored = true)]
        public IList<IgnoredDisciplineModel> IgnoredDisciplines { get; set; }

        public override string GetSettingKey() => "WorkingTimeRequest_ApproveRequest";

        public override bool IsValid() => RequestId > 0;

        private string ConvertDisciplineItemToXml()
        {
            var disciplineXmlBuilder = new StringBuilder();

            if (IgnoredDisciplines != null && IgnoredDisciplines.Any())
            {
                disciplineXmlBuilder.Append(Constants.BeginRootTag);

                foreach (IgnoredDisciplineModel item in IgnoredDisciplines)
                {
                    disciplineXmlBuilder.AppendFormat(DisciplineTemp, item.WorkDate, item.EmployeeId, item.DisciplineTypeId, item.IsIgnored, item.IsConfirmed);
                }

                disciplineXmlBuilder.Append(Constants.EndRootTag);
            }

            return disciplineXmlBuilder.ToString();
        }
    }
}