namespace Fanex.NexHR.Models
{
    using System;

    [Serializable]
    public class SystemRoleModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}