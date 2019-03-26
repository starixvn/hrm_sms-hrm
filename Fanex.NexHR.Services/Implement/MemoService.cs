namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria.Memo;
    using Models;

    public class MemoService : IMemoService
    {
        private readonly IBaseService baseService;

        public MemoService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<Memo> GetAll(GetAllMemoCriteria criteria)
        {
            IList<Memo> entityList = null;

            if (criteria.IsValid())
            {
                entityList = baseService.GetData<Memo>(criteria).ToList();
            }

            return entityList ?? new List<Memo>();
        }

        public Memo GetById(GetMemoByIdCriteria criteria)
        {
            var memo = baseService.GetDataEntity<Memo>(criteria);

            if (memo != null && memo.MemoType == 1)
            {
                memo.AttachFile = memo.Contents;
            }

            return memo ?? new Memo();
        }
    }
}