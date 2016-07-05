namespace Fanex.NexHR.Repositories
{
    using Criteria.Memo;
    using Models;
    using Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class MemoService : BaseService, IMemoService
    {
        public IList<Memo> GetAll(GetAllMemoCriteria criteria)
        {
            IList<Memo> entityList = null;

            if (criteria.IsValid())
            {
                entityList = GetData<Memo>(criteria).ToList();
            }

            return entityList ?? new List<Memo>();
        }

        public Memo GetById(GetMemoByIdCriteria criteria)
        {
            var memo = GetDataEntity<Memo>(criteria);

            if (memo != null && memo.MemoType == 1)
            {
                memo.AttachFile = memo.Contents;
            }

            return memo?? new Memo();
        }
    }
}
