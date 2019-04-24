namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Function;

    public interface IFunctionService
    {
        IEnumerable<FunctionModel> GetFunctions();

        IEnumerable<FunctionModel> GetFunctionsByUserId(int userId);

        IEnumerable<FunctionModel> GetFunctionsByRoleId(int roleId);

        CommandResult AssignFunctionsToRole(int actorId, int roleId, IEnumerable<FunctionPermission> permissions);
    }
}