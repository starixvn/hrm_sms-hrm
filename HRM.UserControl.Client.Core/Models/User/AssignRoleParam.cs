namespace HRM.UserControl.Client.Core.Models.User
{
    using System;

    [Serializable]
    public class AssignRoleParam
    {
        public int UserId { get; set; }
        public string RoleIds { get; set; }
    }
}