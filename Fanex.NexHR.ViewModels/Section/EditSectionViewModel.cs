namespace Fanex.NexHR.ViewModels.Section
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditSectionViewModel : AddSectionViewModel
    {
        [Required]
        public int SectionId { get; set; }
    }
}
