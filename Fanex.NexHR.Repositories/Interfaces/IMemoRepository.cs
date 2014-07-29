// --------------------------------------------------------------------------
// <copyright file="IMemoRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IMemoRepository
    {
        Memo GetById(int id);

        int Insert(Memo model);

        bool Delete(int id);

        bool Update(Memo model);
    }
}
