namespace Fanex.NexHR.ViewModels.Function
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class TreeFunctionViewModel
    {
        public int RoleId { get; set; }
        public bool IsHost { get; set; }
        public IEnumerable<FunctionModel> Functions { get; set; }
    }
}