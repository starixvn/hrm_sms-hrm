namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public interface IUnitService
    {
        Unit GetById(int id);

        IList<Unit> GetUnit(bool? isActive = true);

    }
}