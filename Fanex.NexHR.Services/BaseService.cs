namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;

    public class BaseService
    {
        private readonly IBaseRepository repository;
        private readonly IDynamicRepository dynamicRepository;

        public BaseService()
            : this(null, null)
        {
        }

        public BaseService(IDynamicRepository dynamicRepository, IBaseRepository repository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository
        {
            get
            {
                return repository;
            }
        }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }
    }
}
