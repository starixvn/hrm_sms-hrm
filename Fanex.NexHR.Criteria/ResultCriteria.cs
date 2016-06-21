// -----------------------------------------------------------------------
// <copyright file="ResultCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.16">Created</change>
// </history>

namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// The Result Criteria.
    /// </summary>
    public class ResultCriteria : CriteriaBase
    {
        public ResultCriteria()
        {
            ErrCode = 0;
            ErrMsg = string.Empty;
        }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}
