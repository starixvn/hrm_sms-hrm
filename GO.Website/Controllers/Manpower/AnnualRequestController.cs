namespace GO.Website.Controllers.Manpower
{
    using System.Threading.Tasks;
    using GO.Admin.Website.Filters;
    using GO.Core.Models;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Criteria.Manpower.AnnualRequest;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.AnnualRequest;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("ManpowerForecast/AnnualRequest/[action]/{id?}")]
    public class AnnualRequestController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IManpowerForecastService manpowerForecastService;

        public AnnualRequestController(
            IUserControlSession userControlSession,
            IManpowerForecastService manpowerForecastService)
        {
            this.userControlSession = userControlSession;
            this.manpowerForecastService = manpowerForecastService;
        }

        [GOPermission(FunctionKey = FunctionKey.MyAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public async Task<IActionResult> Create()
        {
            var units = await manpowerForecastService.GetUnitByUserIdAsync(userControlSession.UserId);
            return View("~/Views/Manpower/AnnualRequest/Create.cshtml", units);
        }

        [GOPermission(FunctionKey = FunctionKey.MyAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public IActionResult Summary()
        {
            return View("~/Views/Manpower/AnnualRequest/Summary.cshtml");
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.MyAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public async Task<IActionResult> GetDataForCreateAnnualRequest(string unitName)
        {
            (var period, var details) = await manpowerForecastService.GetDataForCreateAnnualRequestAsync(userControlSession.UserId, unitName);
            return Json(new { period, details });
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.MyAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateAnnualManpowerForecastRequestParam param)
        {
            CommandResult commandResult = await manpowerForecastService.CreateAnnualRequestAsync(userControlSession.UserId, param);
            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.MyAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] UpdateAnnualManpowerForecastRequestParam param)
        {
            CommandResult commandResult = await manpowerForecastService.UpdateAnnualRequestAsync(userControlSession.UserId, param);
            return Json(commandResult);
        }

        [GOPermission(FunctionKey = FunctionKey.MyAnnualManpowerForecastRequestForm, Policy = Policy.Read)]
        public IActionResult MyRequest()
        {
            return View("~/Views/Manpower/AnnualRequest/MyRequest.cshtml");
        }

        public IActionResult Detail(int id)
        {
            return View("~/Views/Manpower/AnnualRequest/Detail.cshtml", id);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.MyAnnualManpowerForecastRequestForm, Policy = Policy.Read)]
        [GOPermission(FunctionKey = FunctionKey.MemberAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public async Task<IActionResult> GetMyRequest()
        {
            var requests = await manpowerForecastService.GetMyAnnualRequestAsync(userControlSession.UserId);
            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await manpowerForecastService.GetAnnualRequestByIdAsync(id);
            if (request is null)
            {
                return NotFound();
            }

            request.IsEdit = IsEditIntoUnitHead(request) || IsEditIntoVerify(request) || IsEditIntoApprove(request);
            return Json(request);
        }

        [HttpPost]
        public async Task<IActionResult> GetMemberSummaryRequest([FromBody] GetMemberSummaryAnnualManpowerForecastRequestCriteria criteria)
        {
            criteria.UserId = userControlSession.UserId;
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.MemberSummayAnnualManpowerForecastRequestForm, Policy.Host);
            var requests = await manpowerForecastService.GetAllAnnualRequestByPeriodIdAsync(criteria);
            return Json(new { requests, isHost });
        }

        [GOPermission(FunctionKey = FunctionKey.MemberAnnualManpowerForecastRequestForm, Policy = Policy.Read)]
        public IActionResult MemberRequest()
        {
            return View("~/Views/Manpower/AnnualRequest/MemberRequest.cshtml");
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.MemberAnnualManpowerForecastRequestForm, Policy = Policy.Read)]
        public async Task<IActionResult> GetMemberRequests([FromBody] GetMemberAnnualManpowerForecastRequestCriteria criteria)
        {
            criteria.UserId = userControlSession.UserId;
            var requests = await manpowerForecastService.GetMemberAnnualRequestAsync(criteria);
            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> GetByPeriodId([FromBody] GetAnnualManpowerForecastRequestByPeriodIdCriteria criteria)
        {
            var requests = await manpowerForecastService.GetAnnualRequestByPeriodIdAsync(criteria);
            return Json(requests);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.MemberAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusAnnualManpowerForecastRequestCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var requests = await manpowerForecastService.UpdateStatusAnnualRequestAsync(command);
            return Json(requests);
        }


        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.MemberSummayAnnualManpowerForecastRequestForm, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatusMemberSummaryRequest([FromBody] UpdateStatusMemberSummaryRequestAnnualManpowerForecastRequestCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var requests = await manpowerForecastService.UpdateStatusAnnualRequestByPeriodIdAsync(command);
            return Json(requests);
        }

        private bool IsEditIntoUnitHead(AnnualManpowerForecastRequest request)
            => request.Creator.CreatorId == userControlSession.UserId && request.CurrentProcessId == PeriodProcess.UnitHead;

        private bool IsEditIntoVerify(AnnualManpowerForecastRequest request)
           => request.Verifier.UserId == userControlSession.UserId && request.CurrentProcessId == PeriodProcess.Verifier;

        private bool IsEditIntoApprove(AnnualManpowerForecastRequest request)
           => request.Approver.UserId == userControlSession.UserId && request.CurrentProcessId == PeriodProcess.Approver;
    }
}