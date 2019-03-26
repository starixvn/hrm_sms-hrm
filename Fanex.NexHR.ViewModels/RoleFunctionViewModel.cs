namespace Fanex.NexHR.ViewModels
{
    using Fanex.NexHR.Models;

    public class RoleFunctionViewModel
    {
        public int RoleId { get; set; }

        public FunctionModel[] Functions { get; set; }
    }
}