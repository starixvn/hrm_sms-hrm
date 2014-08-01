// -----------------------------------------------------------------------
// <copyright file="ProcessSingleOvertimeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.8.1">Created.</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.Overtime
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// Process overtime criteria.
    /// </summary>
    public class ProcessSingleOvertimeCriteria : CriteriaBase
    {
        public long RequestID { get; set; }

        public int ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public int RoleID { get; set; }

        [SpParam(SqlParamName = "bOnbehalf")]
        public bool Onbehalf { get; set; }

        [SpParam(Ignored = true)]
        public string CommandProcess { get; set; }

        public override string GetSettingKey()
        {
            return GetSpKeyForProcessSingleOvertime(CommandProcess);
        }

        public override bool IsValid()
        {
            return ProcessedBy > 0 && RequestID > 0;
        }

        /// <summary>
        /// Gets the key for process request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The key name for processing request.</returns>
        private string GetSpKeyForProcessSingleOvertime(string command)
        {
            string key = string.Empty;

            if (command == Command.Accept.ToString()
                || command == Command.Approve.ToString()
                || command == Command.Process.ToString())
            {
                key = "OverTime_ApproveSingleRequest";
            }
            else if (command == Command.NotAccept.ToString()
                || command == Command.Reject.ToString()
                || command == Command.Void.ToString())
            {
                key = "OverTime_RejectSingleRequest";
            }
            else
            {
                key = "Overtime_ResetSingleRequest";
            }

            return key;
        }
    }
}
