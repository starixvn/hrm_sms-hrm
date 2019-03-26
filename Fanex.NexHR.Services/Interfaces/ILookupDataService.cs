namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;

    public interface ILookupDataService
    {
        LookupData GetLookupByCodeAndId(string lookupCode, int lookupId);

        IList<LookupData> GetAll(GetAllLookupDataCriteria criteria);

        IList<LookupData> GetJobTitle(GetJobTitleCriteria criteria);
    }
}