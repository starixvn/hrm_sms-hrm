namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Services;
    using HRM.UserControl.API.Core.Services.Interfaces;
    using HRM.UserControl.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Authorize]
    public class UserController : BaseController
    {
        private readonly IRoleService roleService;
        private readonly IJwtManagerService jwtManagerService;
        private readonly IUserService userService;

        public UserController(IRoleService roleService,
            IJwtManagerService jwtManagerService,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this.roleService = roleService;
            this.jwtManagerService = jwtManagerService;
            this.userService = userService;
        }

        [HttpGet]
        [Route("api/user/getById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            User user = await userService.GetByIdAsync(userId);

            return Ok(user);
        }

        [HttpGet]
        [Route("api/user/getRolesOnSite")]
        public async Task<IActionResult> GetCompanyRolesOnSite()
        {
            var companyRoles = await roleService.GetUserRolesOnSite(UserId, SiteId);
            return Ok(companyRoles);
        }

        [HttpGet]
        [Route("api/user/getRoleOnCompany")]
        public async Task<IActionResult> GetRoleOnCompany(int companyId)
        {
            var companyRoles = await roleService.GetUserRolesOnSite(UserId, SiteId);
            var companyRole = companyRoles.FirstOrDefault(company => company.CompanyId == companyId);
            return View(companyRole);
        }

        [HttpPost]
        [Route("api/user/companies")]
        public async Task<IActionResult> GetUserCompanyList()
        {
            var companies = await userService.GetUserCompanyBySiteAsync(UserId, SiteId);
            return Ok(companies);
        }

        [HttpPost]
        [Route("api/refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            string token = await jwtManagerService.GenerateToken(UserId, SiteId);

            return Ok(JsonConvert.SerializeObject(token));
        }
    }
}