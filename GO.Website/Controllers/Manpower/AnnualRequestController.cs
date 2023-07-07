namespace GO.Website.Controllers.Manpower
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Admin.Website.Filters;
    using GO.Core.Models;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.AnnualRequest;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("ManpowerForecast/AnnualRequest/[action]/{id?}")]
    [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_AnnualRequest, Policy = Policy.Host)]
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

        public async Task<IActionResult> Create()
        {
            var units = await manpowerForecastService.GetUnitByUserIdAsync(userControlSession.UserId);
            return View("~/Views/Manpower/AnnualRequest/Create.cshtml", units);
        }

        [HttpPost]
        public async Task<IActionResult> GetDataForCreateAnnualRequest(string unitName)
        {
            (var period, var details) = await manpowerForecastService.GetDataForCreateAnnualRequestAsync(userControlSession.UserId, unitName);
            var departments = GetDepartments(details);
            return Json(new { period, departments, details });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnnualManpowerForecastRequestParam param)
        {
            CommandResult commandResult = await manpowerForecastService.CreateAnnualRequestAsync(userControlSession.UserId, param);
            return Json(commandResult);
        }

        public IActionResult MyRequest()
        {
            return View("~/Views/Manpower/AnnualRequest/MyRequest.cshtml");
        }

        public IActionResult Detail(int id)
        {
            return View("~/Views/Manpower/AnnualRequest/Detail.cshtml", id);
        }

        [HttpPost]
        public async Task<IActionResult> GetMyRequest()
        {
            var requests = await manpowerForecastService.GetMyAnnualRequestAsync(userControlSession.UserId);
            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> GetAnnualRequestById(int requestId)
        {
            var request = await manpowerForecastService.GetAnnualRequestByIdAsync(requestId);
            if(request is null)
            {
                return NotFound();
            }
            request.IsEdit = IsEditRequest(request);
            var departments = GetDepartments(request.Details);
            return Json(new { request, departments});
        }

        public IActionResult MemberRequest()
        {
            return View("~/Views/Manpower/AnnualRequest/MemberRequest.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetMemberRequests([FromBody] GetMemberAnnualManpowerForecastRequestCriteria criteria)
        {
            criteria.UserId = userControlSession.UserId;
            var requests = await manpowerForecastService.GetMemberAnnualRequestAsync(criteria);
            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> GetAnnualRequestByPeriodId([FromBody] GetAnnualManpowerForecastRequestByPeriodIdCriteria criteria)
        {
            var requests = await manpowerForecastService.GetAnnualRequestByPeriodIdAsync(criteria);
            return Json(requests);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusAnnualManpowerForecastRequestCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var requests = await manpowerForecastService.UpdateStatusAnnualRequestAsync(command);
            return Json(requests);
        }

        private static IList<Department> GetDepartments(IEnumerable<AnnualManpowerForecastRequestDetail> details)
        {
            var deparments = new List<Department>();
            foreach (var item in details)
            {
                var IsValidDepartment = deparments.Where(x => x.DepartmentName == item.DepartmentName).ToList();
                if (!IsValidDepartment.Any())
                {
                    deparments.Add(new Department()
                    {
                        DepartmentName = item.DepartmentName,
                        Sections = details.Where(x => x.DepartmentName == item.DepartmentName)
                                          .Select(y => y.SectionName).Distinct().ToList()
                    });
                }
            }
            return deparments;
        }

        public bool IsEditRequest(AnnualManpowerForecastRequest request)
        {
            if (request.Creator.CreatorId == userControlSession.UserId && request.CurrentProcessId == PeriodProcess.UnitHead) {
                return true;
            }

            if (request.Verifier.UserId == userControlSession.UserId && request.CurrentProcessId == PeriodProcess.Verify)
            {
                return true;
            }

            if (request.Approver.UserId == userControlSession.UserId && request.CurrentProcessId == PeriodProcess.Approve)
            {
                return true;
            }

            return false;
        }
    }
}