namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;

    public class SectionService : ISectionService
    {
        private readonly ISectionRepository sectionRepository;

        public SectionService(
            ISectionRepository sectionRepository)
        {
            this.sectionRepository = sectionRepository;
        }

        public Section GetById(int id)
        {
            var criteria = new GetSectionByIdCriteria
            {
                SectionId = id
            };
            return sectionRepository.GetById(criteria);
        }

        public IList<Section> GetSection(int? parentId = null, bool? isActive = true)
        {
            var criteria = new GetSectionByParentIdCriteria
            {
                ParentId = parentId,
                IsActive = isActive
            };
            IList<Section> sections = sectionRepository.GetSection(criteria);

            return sections ?? new List<Section>();
        }

    }
}