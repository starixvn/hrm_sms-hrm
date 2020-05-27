namespace Fanex.NexHR.ViewModels.Unit
{
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AddUnitViewModel
    {
        private const int MaxSizeCode = 20;
        private const int MaxSizeName = 150;

        public AddUnitViewModel()
        {
            IsActive = true;
        }

        [AllowHtml]
        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(MaxSizeCode, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Code), ResourceType = typeof(Labels))]
        public string Code { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
        [StringLength(MaxSizeName, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
        [Display(Name = nameof(Labels.Name), ResourceType = typeof(Labels))]
        public string Name { get; set; }

        public bool IsActive { get; set; }

    }
}
