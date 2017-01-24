namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class ProcessMultipleOvertimeCriteria : CriteriaBase
    {
        public int ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public int RoleId { get; set; }

        [SpParam(SqlParamName = "bOnbehalf")]
        public bool Onbehalf { get; set; }

        public string ListRequestId { get; set; }

        [SpParam(Ignored = true)]
        public string CommandProcess { get; set; }

        public override string GetSettingKey()
        {
            return GetSpKeyForProcessMultipleOvertime(CommandProcess);
        }

        public override bool IsValid()
        {
            return ProcessedBy > 0;
        }

        public int GetRoleIdForProcessOvertime(string command)
        {
            int roleId = 0;

            if (command == Command.Accept.ToString()
                || command == Command.NotAccept.ToString())
            {
                roleId = (int)RoleSystem.Verifier;
            }
            else if (command == Command.Approve.ToString()
                || command == Command.Reject.ToString())
            {
                roleId = (int)RoleSystem.Approver;
            }
            else if (command == Command.Process.ToString()
                || command == Command.Void.ToString())
            {
                roleId = (int)RoleSystem.HR;
            }

            return roleId;
        }

        private string GetSpKeyForProcessMultipleOvertime(string command)
        {
            string key = string.Empty;

            if (command == Command.Accept.ToString()
                || command == Command.Approve.ToString()
                || command == Command.Process.ToString())
            {
                key = "OverTime_ApproveMultiRequest";
            }
            else if (command == Command.NotAccept.ToString()
                || command == Command.Reject.ToString()
                || command == Command.Void.ToString())
            {
                key = "OverTime_RejectMultiRequest";
            }
            else
            {
                key = "Overtime_ResetMultiRequest";
            }

            return key;
        }
    }
}