namespace Fanex.NexHR.ViewModels.Unit
{
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditUnitViewModel
    {
        [Required]
        public int UnitId { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(20, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Code), ResourceType = typeof(Labels))]
        public string Code { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Name), ResourceType = typeof(Labels))]
        public string Name { get; set; }

        public bool IsActive { get; set; }

    }
}
