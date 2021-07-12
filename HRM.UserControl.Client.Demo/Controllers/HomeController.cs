namespace HRM.UserControl.Client.Demo.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Services.Interfaces;

    public class HomeController : Controller
    {
        private readonly IUserService userService;

        public HomeController() : this(null)
        {
        }

        public HomeController(IUserService userService)
        {
            this.userService = userService ?? new UserService();
        }

        public async Task<ActionResult> Index()
        {
            var user = await userService.GetByEmailAsync("oliver.dang@nexcel.vn");
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}