namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IMemoService
    {
        IList<Memo> GetAll(GetAllMemoCriteria criteria);

        Memo GetById(GetMemoByIdCriteria criteria);
    }
}
