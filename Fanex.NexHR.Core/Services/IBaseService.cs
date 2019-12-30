namespace Fanex.NexHR.Core.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Models;

    public interface IBaseService
    {
        IEnumerable<T> GetData<T>(ICriteria criteria);

        T GetDataEntity<T>(ICriteria criteria);

        CommandResult ExecuteNonQuery(ICriteria criteria);
    }
}