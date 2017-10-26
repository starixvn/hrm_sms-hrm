namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class LookupDataService : BaseService, ILookupDataService
    {
        public IList<LookupData> GetAll(GetAllLookupDataCriteria criteria)
        {
            IList<LookupData> lookupDatas = GetData<LookupData>(criteria).ToList();
            return lookupDatas;
        }

        public LookupData GetLookupByCodeAndId(string lookupCode, int lookupId)
        {
            var criteria = new GetAllLookupDataCriteria
            {
                LookupCode = lookupCode
            };
            var lookupDataList = GetAll(criteria);
            LookupData lookupData = lookupDataList.Single(data => data.Id == lookupId);

            return lookupData;
        }

        public IList<LookupData> GetJobTitle(GetJobTitleCriteria criteria)
        {
            IList<LookupData> jobTitles = GetData<LookupData>(criteria).ToList();
            return jobTitles;
        }
    }
}