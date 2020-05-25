namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Unit;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;

    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;

        public UnitService(
            IUnitRepository unitRepository)
        {
            this._unitRepository = unitRepository;
        }

    
        public Unit GetById(int id)
        {
            var criteria = new GetUnitByIdCriteria
            {
                UnitId = id
            };
            return _unitRepository.GetById(criteria);
        }

        public IList<Unit> GetUnit(bool? isActive = true)
        {
            var criteria = new GetUnitsCriteria
            {
                IsActive = isActive
            };
            IList<Unit> units = _unitRepository.GetUnit(criteria);

            return units ?? new List<Unit>();
        }

    }
}