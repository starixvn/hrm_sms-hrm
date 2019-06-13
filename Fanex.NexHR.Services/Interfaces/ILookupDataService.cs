namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface ILookupDataService
    {
        LookupData GetLookupByCodeAndId(string lookupCode, int lookupId);

        IList<LookupData> GetAll(string code);

        IList<LookupData> GetJobTitle(string jobTitle);
    }
}