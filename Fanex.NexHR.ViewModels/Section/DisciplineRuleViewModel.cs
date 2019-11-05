namespace Fanex.NexHR.ViewModels.Section
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Section;

    public class DisciplineRuleViewModel
    {
        public int SectionId { get; set; }
        public string Section { get; set; }
        public bool IsHost { get; set; }
        public IEnumerable<DisciplineRule> DisciplineRules { get; set; } = new List<DisciplineRule>();
    }
}