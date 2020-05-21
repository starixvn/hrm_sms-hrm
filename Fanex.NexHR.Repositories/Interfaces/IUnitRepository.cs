namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IUnitRepository
    {
        Unit GetById(ICriteria criteria);

        IList<Unit> GetUnit(ICriteria criteria);
    }
}