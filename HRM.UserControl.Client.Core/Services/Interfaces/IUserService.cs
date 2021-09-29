namespace HRM.UserControl.Client.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IUserService
    {
        User GetById(int userId);

        Task<User> GetByIdAsync(int userId);

        User GetByEmail(string email);

        Task<User> GetByEmailAsync(string email);

        IList<CompanyRoleFunction> GetPermission();

        Task<IList<CompanyRoleFunction>> GetPermissionAsync();

        CompanyRoleFunction GetPermissionOnCompany(int companyId);

        Task<CompanyRoleFunction> GetPermissionOnCompanyAsync(int companyId);

        IList<Company> GetCompany();

        Task<IList<Company>> GetCompanyAsync();
    }
}