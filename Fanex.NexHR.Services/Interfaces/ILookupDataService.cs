// --------------------------------------------------------------------------
// <copyright file="ILookupDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;

    public interface ILookupDataService
    {
        LookupData GetLookupByCodeAndId(string lookupCode, int lookupId);

        IList<LookupData> GetAll(GetAllLookupDataCriteria criteria);

        IEnumerable<LookupData> GetJobTitle(GetJobTitleCriteria criteria);

        CommandResult Delete(DeleteLookupDataCriteria criteria);

        CommandResult Insert(InsertLookupDataCriteria criteria);

        CommandResult Update(UpdateLookupDataCriteria criteria);
    }
}