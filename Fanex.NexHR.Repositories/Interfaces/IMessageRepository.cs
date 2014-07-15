// -----------------------------------------------------------------------
// <copyright file="IMessageRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    /// <summary>
    /// The interface of Message repository.
    /// </summary>
    public interface IMessageRepository
    {
        IEnumerable<Message> GetMessageByCreator(ICriteria criteria);
    }
}
