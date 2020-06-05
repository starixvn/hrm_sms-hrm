namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ISectionService
    {
        Section GetById(int id);

        IList<Section> GetSection(bool? isActive = true);
    }
}