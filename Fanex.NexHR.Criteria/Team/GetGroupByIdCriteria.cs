// -----------------------------------------------------------------------
// <copyright file="GetGroupByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Team
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get group by Id criteria.
    /// </summary>
    public class GetGroupByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public bool? IsDeleted { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Select";
        }

        public override bool IsValid()
        {
            return GroupId > 0;
        }
    }
}
