namespace GO.Website.Controllers
{
    using GO.Website.Filters;
    using Microsoft.AspNetCore.Mvc;

    [GOAuthorize]
    public class BaseController : Controller
    {
    }
}