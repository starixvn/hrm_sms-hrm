namespace Fanex.NexHR.Models
{
    using System;

    [Serializable]
    public class SystemRoleModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleCode { get; set; }
    }
}