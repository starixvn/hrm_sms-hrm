namespace Fanex.NexHR.ViewModels.Section
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class SectionViewModel
    {
        public bool? IsActive { get; set; }

        public IList<Section> Sections { get; set; }

        public int ParentId { get; set; }
    }
}
