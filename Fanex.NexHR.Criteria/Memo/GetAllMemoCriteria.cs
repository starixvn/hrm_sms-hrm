// -----------------------------------------------------------------------
// <copyright file="GetAllMemoCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.29">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAllMemoCriteria : CriteriaBase
    {
        public string Content { get; set; }

        public int Year { get; set; }

        public int Category { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
