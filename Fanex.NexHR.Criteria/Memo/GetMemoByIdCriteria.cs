// -----------------------------------------------------------------------
// <copyright file="GetMemoByIdCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.30">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get memo by Id.
    /// </summary>
    public class GetMemoByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_GetById";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
