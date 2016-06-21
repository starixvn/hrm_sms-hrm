// --------------------------------------------------------------------------
// <copyright file="IMemoRepository.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;

    public interface IMemoRepository
    {
        int Insert(ICriteria criteria);

        bool Update(ICriteria criteria);

        bool Delete(ICriteria criteria);
    }
}
