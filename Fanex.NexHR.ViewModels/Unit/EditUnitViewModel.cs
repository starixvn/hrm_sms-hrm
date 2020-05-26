namespace Fanex.NexHR.ViewModels.Unit
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditUnitViewModel
    {
        [Required]
        public int UnitId { get; set; }

        [AllowHtml]
        [Required]
        public string Code { get; set; }

        [AllowHtml]
        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; }

    }
}
