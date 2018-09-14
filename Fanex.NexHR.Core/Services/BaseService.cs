namespace Fanex.NexHR.Core.Services
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public class BaseService
    {
        private readonly IDynamicRepository dynamicRepository;

        public BaseService()
            : this(null)
        {
        }

        public BaseService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
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