namespace Fanex.NexHR.ViewModels.Unit
{
    using System.ComponentModel.DataAnnotations;

    public class AddUnitViewModel
    {
        public AddUnitViewModel()
        {
            IsActive = true;
        }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; }

    }
}
