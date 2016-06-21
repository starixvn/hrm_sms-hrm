// -----------------------------------------------------------------------
// <copyright file="DeleteMemoCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.30">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Delete Memo criteria.
    /// </summary>
    public class DeleteMemoCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
