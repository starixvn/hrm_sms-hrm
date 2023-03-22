namespace GO.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Models.UserMapping;
    using GO.WebAPI.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UserMappingController : BaseController
    {
        private readonly IUserMappingService userMappingService;

        public UserMappingController(
            IHttpContextAccessor httpContextAccessor,
            IUserMappingService userMappingService) : base(httpContextAccessor)
        {
            this.userMappingService = userMappingService;
        }

        [HttpGet]
        [Route("api/UserMapping/GetBySiteId")]
        public async Task<IActionResult> GetBySiteId(DateTime? lastModifiedDate)
        {
            IEnumerable<UserMapping> userMappings = await userMappingService.GetBySiteIdAsync(SiteId, lastModifiedDate);

            return Json(userMappings);
        }
    }
}