using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.RoleManagement.Services
{
    public interface IMenuService
    {
        IEnumerable<MenuModel> GetMenusByUserId(int userId);
    }
}
