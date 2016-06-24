using System.Collections.Generic;

namespace Nexus.RoleManagement.Services
{
    public interface IMenuService
    {
        IEnumerable<MenuModel> GetMenusByUserId(int userId);

        IEnumerable<MenuViewModel> GetMenus();

        Result Insert(InsertMenuCriteria criteria);

        MenuModel GetMenusById(int id);

        Result Update(UpdateMenuCriteria criteria);

        Result Delete(int id);
    }
}
