// -----------------------------------------------------------------------
// <copyright file="UpdateMemoCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.30">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Memo
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Memo criteria.
    /// </summary>
    public class UpdateMemoCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string RefNo { get; set; }

        public string Contents { get; set; }

        public string Subject { get; set; }

        public string SentTo { get; set; }

        public string SentFrom { get; set; }

        public string ForInfo { get; set; }

        public int TotalPages { get; set; }

        public DateTime SentDate { get; set; }

        public int Category { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int MemoType { get; set; }

        [SpParam(Ignored = true)]
        public bool SendMail { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_Update";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
