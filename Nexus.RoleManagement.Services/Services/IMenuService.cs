namespace Nexus.RoleManagement.Services
{
    using System.Collections.Generic;

    public interface IMenuService
    {
        IEnumerable<MenuModel> GetMenusByUserId(int userId);

        IEnumerable<MenuViewModel> GetMenus();

        MenuModel GetMenusById(int id);

        Result Insert(InsertMenuCriteria criteria);

        Result Update(UpdateMenuCriteria criteria);

        Result Delete(int id);
    }
}