namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public class BaseRoleService
    {
        private readonly IBaseRepository _repository;
        private readonly IDynamicRepository _dynamicRepository;

        public BaseRoleService(IDynamicRepository dynamicRepository = null, IBaseRepository repository = null)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            return _dynamicRepository.Fetch<T>(criteria);
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            return _dynamicRepository.Get<T>(criteria);
        }
    }
}