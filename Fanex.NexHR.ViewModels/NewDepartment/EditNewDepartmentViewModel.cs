namespace Fanex.NexHR.ViewModels.NewDepartment
{
    using System.ComponentModel.DataAnnotations;

    public class EditNewDepartmentViewModel : CreateDepartmentViewModel
    {
        [Required]
        public int DepartmentId { get; set; }
    }
}
