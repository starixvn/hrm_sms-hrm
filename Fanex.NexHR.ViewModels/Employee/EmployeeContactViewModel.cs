namespace Fanex.NexHR.ViewModels.Employee
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Fanex.NexHR.Resources;

    public class EmployeeContactViewModel
    {
        [Display(Name = "Chinese Name")]
        [StringLength(200, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        public string ChineseName { get; set; }

        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Email is invalid")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        public string Email { get; set; }

        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Phone number is invalid")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        public string Phone { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        public string Skype { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        [Display(Name = "Let's Talk")]
        public string LetsTalk { get; set; }

        [StringLength(250, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        public string Address { get; set; }

        [StringLength(250, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.ValidateStringLengthMessage))]
        public string CurrentAddress { get; set; }

        public IEnumerable<HttpPostedFileBase> Attachments { get; set; }

        [Display(Name = "Uploaded file(s)")]
        public IEnumerable<EmployeeFileUpload> FileUploaded { get; set; }
    }
}