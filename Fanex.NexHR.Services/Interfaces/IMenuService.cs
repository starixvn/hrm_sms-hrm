namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IMenuService
    {
        IEnumerable<MenuViewModel> GetMenus();

        MenuModel GetMenusById(int id);

        CommandResult Create(MenuModel model);

        CommandResult Edit(MenuModel model);

        CommandResult Delete(int id);
    }
}