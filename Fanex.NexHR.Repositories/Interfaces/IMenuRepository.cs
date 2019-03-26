namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.ViewModels;

    public interface IMenuRepository
    {
        IEnumerable<MenuViewModel> GetMenus(ICriteria criteria);
    }
}