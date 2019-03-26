namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;

    public interface IMemoService
    {
        IList<Memo> GetAll(GetAllMemoCriteria criteria);

        Memo GetById(GetMemoByIdCriteria criteria);
    }
}