// --------------------------------------------------------------------------
// <copyright file="IMemoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;

    public interface IMemoService
    {
        bool Delete(DeleteMemoCriteria criteria);

        IEnumerable<Memo> GetAll(GetAllMemoCriteria criteria);

        Memo GetById(GetMemoByIdCriteria criteria);

        int Insert(InsertMemoCriteria criteria);

        bool Update(UpdateMemoCriteria criteria);
    }
}
