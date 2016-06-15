using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.RoleManagement.Services
{
    public interface IFunctionService
    {
        IEnumerable<FunctionModel> GetFunctions();

        IEnumerable<FunctionModel> GetFunctionsByUserId(int userId);

        IEnumerable<FunctionModel> GetFunctionsByRoleId(int roleId);

        Result AssignFunctionsToRole(AssignFunctionToRoleCriteria criteria);
    }
}
