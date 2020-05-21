namespace Fanex.NexHR.ViewModels.NewDepartment
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditNewDepartmentViewModel
    {
        [Required]
        public int DepartmentId { get; set; }

        [AllowHtml]
        [Required]
        public string Code { get; set; }

        [AllowHtml]
        [Required]
        public string Name { get; set; }

        [Required]
        public int UnitId { get; set; }

        public bool IsActive { get; set; }

        //public IList<Unit> Unit { get; set; }
    }
}
