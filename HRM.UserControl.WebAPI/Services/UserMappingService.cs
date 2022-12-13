namespace HRM.UserControl.WebAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models.UserMapping;
    using HRM.UserControl.WebAPI.Criteria.UserMapping;

    public class UserMappingService : IUserMappingService
    {
        private readonly IBaseService baseService;

        public UserMappingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<UserMapping>> GetBySiteIdAsync(short siteId, DateTime? lastModifiedDate = null)
        {
            var criteria = new GetUserMappingBySiteIdCriteria
            {
                SiteId = siteId,
                LastModifiedDate = lastModifiedDate
            };
            return await baseService.GetDataAsync<UserMapping>(criteria);
        }
    }
}