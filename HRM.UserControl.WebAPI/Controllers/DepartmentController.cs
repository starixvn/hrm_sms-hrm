
namespace HRM.UserControl.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models.Department;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IBaseService baseService;

        public DepartmentController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        [HttpPost]
        public IActionResult Update(Department[] departments)
        {
            return Ok();
        }
    }
}
