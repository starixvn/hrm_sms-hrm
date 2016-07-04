namespace Fanex.NexHR.ViewModels.Employee
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class ChangePasswordViewModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
