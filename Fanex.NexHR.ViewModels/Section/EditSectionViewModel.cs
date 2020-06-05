namespace Fanex.NexHR.ViewModels.Section
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditSectionViewModel
    {
        [Required]
        public int SectionId { get; set; }

        [AllowHtml]
        [Required]
        public string Code { get; set; }

        [AllowHtml]
        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public bool IsActive { get; set; }

        public IList<Section> ParentSections { get; set; }
    }
}
