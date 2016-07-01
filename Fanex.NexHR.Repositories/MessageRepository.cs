namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

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
