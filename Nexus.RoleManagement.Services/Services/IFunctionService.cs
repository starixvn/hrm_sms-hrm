using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.RoleManagement.Services
{
    public interface IFunctionService
    {
        IEnumerable<FunctionModel> GetFunctions();

        IEnumerable<FunctionViewModel> GetFunctionsByUserId(int userId);

        IEnumerable<FunctionViewModel> GetFunctionsByRoleId(int roleId);

        Result AssignFunctionsToRole(AssignFunctionToRoleCriteria criteria);
    }
}
