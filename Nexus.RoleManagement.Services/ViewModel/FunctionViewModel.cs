using System;
namespace Nexus.RoleManagement.Services
{
    [Serializable]
    public class FunctionViewModel : FunctionModel
    {
        public bool IsRead { get; set; }

        public bool IsHost { get; set; }
    }
}
