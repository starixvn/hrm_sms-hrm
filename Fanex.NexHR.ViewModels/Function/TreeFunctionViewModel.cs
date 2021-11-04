namespace Fanex.NexHR.ViewModels.Function
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Models;

    public class TreeFunctionViewModel
    {
        public int RoleId { get; set; }
        public bool IsHost { get; set; }
        public IList<Function> Functions { get; set; } = new List<Function>();
        public IList<FunctionPermission> FunctionPermissions { get; set; } = new List<FunctionPermission>();
    }
}