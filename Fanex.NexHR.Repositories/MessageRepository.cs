// -----------------------------------------------------------------------
// <copyright file="MessageRepository.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// The implement of Message repository.
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        public IEnumerable<Message> GetMessageByCreator(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<Message>(criteria.GetSpParams());
            }
        }
    }
}
