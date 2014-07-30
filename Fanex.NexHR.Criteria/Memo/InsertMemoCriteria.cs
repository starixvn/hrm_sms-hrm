// -----------------------------------------------------------------------
// <copyright file="InsertMemoCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    /// Insert Memo criteria.
    /// </summary>
    public class InsertMemoCriteria : CriteriaBase
    {
        public string RefNo { get; set; }

        public string Contents { get; set; }

        public string Subject { get; set; }

        public string SentTo { get; set; }

        public string SentFrom { get; set; }

        public string ForInfo { get; set; }

        public int TotalPages { get; set; }

        public DateTime SentDate { get; set; }

        public int Category { get; set; }

        public int Type { get; set; }

        [SpParam(Ignored  = true)]
        public bool SendMail { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
