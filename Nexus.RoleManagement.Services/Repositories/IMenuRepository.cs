using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.RoleManagement.Services
{
    public interface IMenuRepository
    {
        IEnumerable<MenuViewModel> GetMenus(ICriteria criteria);
    }
}
