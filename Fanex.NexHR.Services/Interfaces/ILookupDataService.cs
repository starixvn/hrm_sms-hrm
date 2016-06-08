namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface ILookupDataService
    {
        LookupData GetLookupByCodeAndId(string lookupCode, int lookupId);

        IList<LookupData> GetAll(GetAllLookupDataCriteria criteria);

        IList<LookupData> GetJobTitle(GetJobTitleCriteria criteria);
    }
}