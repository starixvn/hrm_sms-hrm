namespace Fanex.NexHR.Core.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Repositories;

    public class BaseService : IBaseService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBaseRepository baseRepository;

        public BaseService(
            IDynamicRepository dynamicRepository,
            IBaseRepository baseRepository)
        {
            this.dynamicRepository = dynamicRepository;
            this.baseRepository = baseRepository;
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

        public virtual CommandResult ExecuteNonQuery(ICriteria criteria, bool hasGetOutputParams = false)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = baseRepository.ExecuteNonQuery(criteria, hasGetOutputParams);
            }

            return commandResult;
        }
    }
}