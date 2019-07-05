namespace Fanex.NexHR.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class EditMonthlyOvertimeRequestStatusCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int RoleId => GetRoleId(CommandProcess);
        public string RequestIds { get; set; }

        [SpParam(Ignored = true)]
        public string CommandProcess { get; set; }

        public override string GetSettingKey() => GetSpKey(CommandProcess);

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(RequestIds);

        public static int GetRoleId(string command)
        {
            int roleId = 0;

            if (command == Command.Accept.ToString() || command == Command.NotAccept.ToString())
            {
                roleId = (int)RoleSystem.Verifier;
            }

            if (command == Command.Approve.ToString() || command == Command.Reject.ToString())
            {
                roleId = (int)RoleSystem.Approver;
            }

            if (command == Command.Process.ToString() || command == Command.Void.ToString())
            {
                roleId = (int)RoleSystem.HR;
            }

            return roleId;
        }

        private static string GetSpKey(string command)
        {
            string key = string.Empty;

            if (command == Command.Accept.ToString()
                || command == Command.Approve.ToString()
                || command == Command.Process.ToString())
            {
                key = "Overtime_ApproveRequest";
            }
            else if (command == Command.NotAccept.ToString()
                || command == Command.Reject.ToString()
                || command == Command.Void.ToString())
            {
                key = "Overtime_RejectRequest";
            }
            else
            {
                key = "Overtime_ResetRequest";
            }

            return key;
        }
    }
}