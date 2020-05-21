namespace Fanex.NexHR.ViewModels.NewDepartment
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class CreateDepartmentViewModel
    {
        public CreateDepartmentViewModel()
        {
            IsActive = true;
        }

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
