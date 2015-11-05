// --------------------------------------------------------------------------
// <copyright file="GetManpowerCategoryCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.4">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetManpowerCategoryCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Report_ManpowerCategory";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}
