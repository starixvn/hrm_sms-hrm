namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IMemoService
    {
        IEnumerable<Memo> GetAll(GetAllMemoCriteria criteria);

        Memo GetById(GetMemoByIdCriteria criteria);

        CommandResult Insert(InsertMemoCriteria criteria);

        CommandResult Update(UpdateMemoCriteria criteria);

        CommandResult Delete(DeleteMemoCriteria criteria);
    }
}
