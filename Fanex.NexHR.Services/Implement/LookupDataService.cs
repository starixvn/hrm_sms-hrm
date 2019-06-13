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

        public IList<LookupData> GetAll(string code)
        {
            var criteria = new GetAllLookupDataCriteria
            {
                LookupCode = code
            };
            return baseService.GetData<LookupData>(criteria).ToList();
        }

        public LookupData GetLookupByCodeAndId(string lookupCode, int lookupId)
        {
            IList<LookupData> lookupDatas = GetAll(lookupCode);
            LookupData lookupData = lookupDatas.Single(data => data.Id == lookupId);

            return lookupData;
        }

        public IList<LookupData> GetJobTitle(string jobTitle)
        {
            var criteria = new GetJobTitleCriteria
            {
                JobTitle = jobTitle
            };
            return baseService.GetData<LookupData>(criteria).ToList();
        }
    }
}