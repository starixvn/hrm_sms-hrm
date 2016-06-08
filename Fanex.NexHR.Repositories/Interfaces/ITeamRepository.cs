namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface ITeamRepository
    {
        Team GetById(ICriteria criteria);

        IList<Team> GetAll(ICriteria criteria);
    }
}
