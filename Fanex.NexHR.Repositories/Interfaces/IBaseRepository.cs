// -----------------------------------------------------------------------
// <copyright file="IBaseRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.16">Created</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    /// <summary>
    /// The interface of Base Repository.
    /// </summary>
    public interface IBaseRepository
    {
        CommandResult Insert(ICriteria criteria);

        CommandResult Update(ICriteria criteria);

        CommandResult Delete(ICriteria criteria);
    }
}
