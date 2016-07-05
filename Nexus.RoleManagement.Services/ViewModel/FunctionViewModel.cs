namespace Nexus.RoleManagement.Services
{
    using System;

    [Serializable]
    public class FunctionViewModel : FunctionModel
    {
        public bool IsRead { get; set; }

        public bool IsHost { get; set; }
    }
}