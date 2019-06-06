namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;

    public interface IAuthService
    {
        HashProvider HashProvider { get; }

        EmployeeSession GetEmployeeByCode(string code);

        int Validate(EmployeeSession employee, string password);

        IEnumerable<FunctionSession> GetFunctionByEmployeeId(int employeeId);

        IEnumerable<MenuItemSession> GetMenuByEmployeeId(int employeeId);
    }
}