namespace HRM.UserControl.Client.Core.Session
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IUCSession
    {
        int UserId { get; set; }

        int OnCompanyId { get; set; }

        string JwtToken { get; set; }

        IList<CompanyRoleFunction> Roles { get; set; }

        void ClearAll();
    }
}