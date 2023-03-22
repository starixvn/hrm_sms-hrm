namespace GO.Admin.Website.Controllers
{
    using GO.Admin.Website.Filters;
    using Microsoft.AspNetCore.Mvc;

    [UCAuthorize]
    public class BaseController : Controller
    {
    }
}