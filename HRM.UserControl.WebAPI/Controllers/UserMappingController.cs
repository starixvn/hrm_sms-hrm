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
        public async Task<IActionResult> GetBySiteId(string lastModifiedDate)
        {
            DateTime? lastUCModifiedDate = null;
            if (!string.IsNullOrEmpty(lastModifiedDate))
            {
                lastUCModifiedDate = DateTime.ParseExact(lastModifiedDate, "yyyy-MM-dd HH:mm:ss:fff", null);
            }

            var userMappings = await userMappingService.GetBySiteIdAsync(lastUCModifiedDate);
            return Json(userMappings);
        }
    }
}
