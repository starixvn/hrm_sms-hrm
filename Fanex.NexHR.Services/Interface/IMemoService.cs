// --------------------------------------------------------------------------
// <copyright file="IMemoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;

    public interface IMemoService
    {
        bool Delete(int id);

        IEnumerable<Memo> GetAll(GetAllMemoCriteria criteria);

        Fanex.NexHR.Models.Memo GetById(int id);

        int Insert(Fanex.NexHR.Models.Memo entity);

        bool Update(Fanex.NexHR.Models.Memo entity);
    }
}
