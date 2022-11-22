namespace HRM.UserControl.WebAPI.Controllers
{
    using System;
    using System.Threading.Tasks;
    using HRM.UserControl.WebAPI.Services;
    using Microsoft.AspNetCore.Mvc;

    public class UserMappingController : Controller
    {
        private readonly IUserMappingService userMappingService;

        public UserMappingController(
            IUserMappingService userMappingService)
        {
            this.userMappingService = userMappingService;
        }

        [HttpGet]
        [Route("api/UserMapping/GetBySiteId")]
        public async Task<IActionResult> GetBySiteId(short siteId, DateTime lastModifiedDate)
        {
            var userMappings = await userMappingService.GetBySiteIdAsync(siteId, lastModifiedDate);
            return Json(userMappings);
        }
    }
}
