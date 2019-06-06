namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Menu;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IMenuService
    {
        IEnumerable<MenuViewModel> GetMenus();

        MenuModel GetMenusById(int id);

        CommandResult Insert(InsertMenuCriteria criteria);

        CommandResult Update(UpdateMenuCriteria criteria);

        CommandResult Delete(int id);
    }
}