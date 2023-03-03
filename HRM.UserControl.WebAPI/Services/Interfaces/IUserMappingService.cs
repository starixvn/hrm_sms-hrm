namespace HRM.UserControl.WebAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Models.UserMapping;

    public interface IUserMappingService
    {
        Task<IEnumerable<UserMapping>> GetBySiteIdAsync(short siteId, DateTime? lastModifiedDate = null);
    }
}