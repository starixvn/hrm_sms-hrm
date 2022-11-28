namespace HRM.UserControl.WebAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models;
    using HRM.UserControl.Models.UserMapping;
    using HRM.UserControl.WebAPI.Criteria.UserMapping;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class UserMappingService : IUserMappingService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IBaseService baseService;

        public UserMappingService(
            IHttpContextAccessor httpContextAccessor,
            IBaseService baseService
        )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.baseService = baseService;
        }

        public async Task<IEnumerable<UserMapping>> GetBySiteIdAsync(DateTime? lastModifiedDate)
        {
            SiteModel requestSite = GetSiteFromRequestHeader();
            var criteria = new GetUserMappingBySiteIdCriteria
            {
                SiteId = requestSite.SiteId,
                LastModifiedDate = lastModifiedDate
            };
            return await baseService.GetDataAsync<UserMapping>(criteria);
        }

        private SiteModel GetSiteFromRequestHeader()
        {
            string jwtToken = httpContextAccessor.HttpContext.Request.Headers["SiteIdentity"].ToString();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken.Replace("Bearer ", string.Empty));
            var siteJson = token.Payload.SerializeToJson();
            return JsonConvert.DeserializeObject<SiteModel>(siteJson);
        }
    }
}
