namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Auth;

    public class AuthService : IAuthService
    {
        private readonly IBaseService baseService;

        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public HashProvider HashProvider => HashProvider.Sha1;

        public EmployeeSession GetEmployeeByCode(string code)
        {
            var criteria = new GetEmployeeLoginByCodeCriteria
            {
                Code = code
            };
            return baseService.GetDataEntity<EmployeeSession>(criteria);
        }

        public int Validate(EmployeeSession employee, string password)
        {
            //// -1: wrong username
            ////  1: deleted
            ////  2: disabled
            ////  3: wrong password
            ////  0: success

            if (employee == null)
            {
                return -1;
            }

            if (!employee.Enabled)
            {
                return 2;
            }

            if (Encryptor.CompareStringToHash(HashProvider, password, employee.Password) ||
                (ConfigSettings.AllowAdminLogin && password == Constants.PassWordAdmin))
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(employee.Password) || !ConfigSettings.AllowAdminLogin || password != Constants.PassWordAdmin)
            {
                return 3;
            }

            return 1;
        }

        public IEnumerable<FunctionSession> GetFunctionByEmployeeId(int employeeId)
        {
            var criteria = new GetFunctionByUserIdCriteria
            {
                UserId = employeeId
            };
            return baseService.GetData<FunctionSession>(criteria);
        }

        public IEnumerable<MenuItemSession> GetMenuByEmployeeId(int employeeId)
        {
            var criteria = new GetMenusByUserIdCriteria
            {
                UserId = employeeId
            };
            IEnumerable<MenuItemSession> menus = baseService.GetData<MenuItemSession>(criteria);
            menus = menus.GroupBy(i => i.Name).Select(g => g.First()).ToList();

            return menus;
        }
    }
}