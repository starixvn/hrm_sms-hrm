namespace Fanex.NexHR.Core.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;

    public interface IBaseService
    {
        IEnumerable<T> GetData<T>(ICriteria criteria);

        T GetDataEntity<T>(ICriteria criteria);
    }
}