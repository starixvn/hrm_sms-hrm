namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;

    public class LookupDataService : ILookupDataService
    {
        private readonly IBaseService baseService;

        public LookupDataService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<LookupData> GetAll(GetAllLookupDataCriteria criteria)
        {
            return baseService.GetData<LookupData>(criteria).ToList();
        }

        public LookupData GetLookupByCodeAndId(string lookupCode, int lookupId)
        {
            var criteria = new GetAllLookupDataCriteria
            {
                LookupCode = lookupCode
            };
            IList<LookupData> lookupDataList = GetAll(criteria);
            LookupData lookupData = lookupDataList.Single(data => data.Id == lookupId);

            return lookupData;
        }

        public IList<LookupData> GetJobTitle(GetJobTitleCriteria criteria)
        {
            return baseService.GetData<LookupData>(criteria).ToList();
        }
    }
}