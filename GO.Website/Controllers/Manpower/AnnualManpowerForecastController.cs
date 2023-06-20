using GO.Core.Services;
using GO.Website.Constants;
using HRM.UserControl.Client.Core.Enumerables;
using HRM.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;

namespace GO.Website.Controllers.Manpower
{
    [Route("Manpower/Forecast/[action]")]
    //[GOPermission(FunctionKey = FunctionKey.Manpower_PeriodManagement, Policy = Policy.Read)]
    public class AnnualManpowerForecastController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;

        public AnnualManpowerForecastController(
            IUserControlSession userControlSession,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_PeriodManagement, Policy.Host);
            return View("~/Views/Manpower/Forecast/Index.cshtml", isHost);
        }
    }
}
