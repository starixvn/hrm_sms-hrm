namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Function;
    using Fanex.NexHR.Models;

    public interface IFunctionService
    {
        IEnumerable<FunctionModel> GetFunctions();

        IEnumerable<FunctionModel> GetFunctionsByUserId(int userId);

        IEnumerable<FunctionModel> GetFunctionsByRoleId(int roleId);

        CommandResult AssignFunctionsToRole(AssignFunctionToRoleCriteria criteria);
    }
}