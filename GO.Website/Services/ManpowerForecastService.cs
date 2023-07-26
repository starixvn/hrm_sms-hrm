namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Criteria.Manpower.AnnualRequest;
    using GO.Website.Models;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.AnnualRequest;
    using GO.Website.Models.Manpower.Period;
    using GO.Website.Repositories;
    using Microsoft.AspNetCore.Hosting;

    public class ManpowerForecastService : IManpowerForecastService
    {
        private const string NotiEditToUnitHeadTemplatePath = "EmailTemplates/ManpowerForecast/NotiEditToUnitHead.txt";

        private readonly AppSettings appSettings;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IManpowerForecastRepository manpowerForecastRepository;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;

        public ManpowerForecastService(
            AppSettings appSettings,
            IWebHostEnvironment hostEnvironment,
            IManpowerForecastRepository manpowerForecastRepository,
            IBaseService baseService,
            IBaseMailService mailService)
        {
            this.appSettings = appSettings;
            this.hostEnvironment = hostEnvironment;
            this.manpowerForecastRepository = manpowerForecastRepository;
            this.baseService = baseService;
            this.mailService = mailService;
        }

        public async Task<IEnumerable<string>> GetUnitByUserIdAsync(int userId)
        {
            var criteria = new GetUnitByUserIdCriteria
            {
                UserId = userId
            };
            return await baseService.GetDataAsync<string>(criteria);
        }

        public async Task<(ManpowerPeriod, IEnumerable<AnnualManpowerForecastRequestDetail>)> GetDataForCreateAnnualRequestAsync(int userId, string unitName)
        {
            ManpowerPeriod period = null;
            var criteria = new GetDataForCreateAnnualManpowerForecastRequestCriteria
            {
                UserId = userId,
                UnitName = unitName
            };
            (var periods, var details) = await baseService.FetchMultipleAsync<ManpowerPeriod, AnnualManpowerForecastRequestDetail>(criteria);
            if (periods.Any())
            {
                period = periods.First();
                IEnumerable<AnnualManpowerForecastRequestDetail> prevforecasts = await GetPreviousApprovedManpowerForecastAsync(period.PeriodId);
                foreach(var item in details)
                {
                    var prevforecast = prevforecasts.FirstOrDefault(x => x.SiteId == item.SiteId && x.SectionId == item.SectionId && x.JobLevel == item.JobLevel);
                    if(prevforecast != null)
                    {
                        item.LocalCurrentHeadcount = prevforecast.LocalCurrentHeadcount;
                        item.ExpatCurrentHeadcount = prevforecast.ExpatCurrentHeadcount;
                        item.LocalApprovedHeadcount = prevforecast.LocalCurrentHeadcount + prevforecast.LocalAddition;
                        item.ExpatApprovedHeadcount = prevforecast.ExpatCurrentHeadcount + prevforecast.ExpatAddition;
                    }
                }
            }

            return (period, details);
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetMyAnnualRequestAsync(int userId)
        {
            var criteria = new GetMyAnnualManpowerForecastRequestCriteria
            {
                UserId = userId
            };
            return await manpowerForecastRepository.GetMyAnnualRequestAsync(criteria);
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetMemberAnnualRequestAsync(GetMemberAnnualManpowerForecastRequestCriteria criteria)
        {
            return await manpowerForecastRepository.GetMemberAnnualRequestAsync(criteria);
        }

        public async Task<AnnualManpowerForecastRequest> GetAnnualRequestByIdAsync(int requestId)
        {
            var criteria = new GetAnnualManpowerForecastRequestByIdCriteria
            {
                RequestId = requestId
            };
            AnnualManpowerForecastRequest request = await manpowerForecastRepository.GetAnnualRequestByIdAsync(criteria);
            if(request != null)
            {
                IEnumerable<AnnualManpowerForecastRequestDetail> prevForecasts = await GetPreviousApprovedManpowerForecastAsync(request.Period.PeriodId);
                foreach (var item in request.Details)
                {
                    var prevForecast = prevForecasts.FirstOrDefault(x => x.SiteId == item.SiteId && x.SectionId == item.SectionId && x.JobLevel == item.JobLevel);
                    if (prevForecast != null)
                    {
                        item.LocalCurrentHeadcount = prevForecast.LocalCurrentHeadcount;
                        item.ExpatCurrentHeadcount = prevForecast.ExpatCurrentHeadcount;
                        item.LocalApprovedHeadcount = prevForecast.LocalCurrentHeadcount + prevForecast.LocalAddition;
                        item.ExpatApprovedHeadcount = prevForecast.ExpatCurrentHeadcount + prevForecast.ExpatAddition;
                    }
                }
            }

            return request;
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetAllAnnualRequestByPeriodIdAsync(GetAllAnnualManpowerForecastRequestByPeriodIdCriteria criteria)
        {
            var (requests, details) = await manpowerForecastRepository.GetAllAnnualRequestByPeriodIdAsync(criteria);
            foreach(var request in requests)
            {
                request.Details = details.Where(x => x.RequestId == request.RequestId).ToList();
                IEnumerable<AnnualManpowerForecastRequestDetail> prevforecasts = await GetPreviousApprovedManpowerForecastAsync(criteria.PeriodId);
                foreach (var detail in request.Details)
                {
                    var prevforecast = prevforecasts.Where(x => x.SiteId == detail.SiteId && x.UnitName == detail.UnitName 
                                                            && x.DepartmentName == detail.DepartmentName && x.SectionId == detail.SectionId).ToList();
                    if (prevforecast != null)
                    {
                        detail.LocalCurrentHeadcount = prevforecast.Sum(x => x.LocalCurrentHeadcount);
                        detail.ExpatCurrentHeadcount = prevforecast.Sum(x => x.ExpatCurrentHeadcount);
                        detail.LocalApprovedHeadcount = prevforecast.Sum(x => x.LocalCurrentHeadcount) + prevforecast.Sum(x => x.LocalAddition);
                        detail.ExpatApprovedHeadcount = prevforecast.Sum(x => x.ExpatCurrentHeadcount) + prevforecast.Sum(x => x.ExpatAddition);
                    }
                }
            }

            return requests;
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequest>> GetAnnualRequestByPeriodIdAsync(GetAnnualManpowerForecastRequestByPeriodIdCriteria criteria)
        {
            return await manpowerForecastRepository.GetAnnualRequestByPeriodIdAsync(criteria);
        }

        public async Task<CommandResult> CreateAnnualRequestAsync(int userId, CreateAnnualManpowerForecastRequestParam param)
        {
            var command = new CreateAnnualManpowerForecastRequestCommand
            {
                PeriodId = param.PeriodId,
                UnitName = param.UnitName,
                DetailXml = param.Details != null && param.Details.Any() ? param.Details.ToXML().ToString() : string.Empty,
                CreatedBy = userId
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UpdateAnnualRequestAsync(int userId, UpdateAnnualManpowerForecastRequestParam param)
        {
            var command = new UpdateAnnualManpowerForecastRequestCommand
            {
                RequestId = param.RequestId,
                DetailXml = param.Details != null && param.Details.Any() ? param.Details.ToXML().ToString() : string.Empty,
                StatusId = param.StatusId,
                ModifiedBy = userId
            };
            var request = await GetAnnualRequestByIdAsync(param.RequestId);
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            if (commandResult.IsSuccessful)
            {
                await SendMailToUnitHeadAsync(request, param);
            }

            return commandResult;
        }

        public async Task<CommandResult> UpdateStatusAnnualRequestAsync(UpdateStatusAnnualManpowerForecastRequestCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UpdateStatusAnnualRequestByPeriodIdAsync(UpdateStatusAnnualManpowerForecastRequestByPeriodIdCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<IEnumerable<AnnualManpowerForecastRequestDetail>> GetPreviousApprovedManpowerForecastAsync(int periodId)
        {
            var criteria = new GetPreviousApprovedManpowerForecastByPeriodIdCriteria
            {
                PeriodId = periodId
            };
            return await baseService.GetDataAsync<AnnualManpowerForecastRequestDetail>(criteria);
        }

        private async Task SendMailToUnitHeadAsync(AnnualManpowerForecastRequest request, UpdateAnnualManpowerForecastRequestParam param)
        {
            if ((param.StatusId == AnnualManpowerForecastRequestStatus.Verified || param.StatusId == AnnualManpowerForecastRequestStatus.Approved)
                && HasEditAddition(request, param))
            {
                ManpowerFlow unitHead = await GetUnitHeadAsync(request.UnitName);
                if (unitHead != null)
                {
                    var templatePath = Path.Combine(hostEnvironment.ContentRootPath, NotiEditToUnitHeadTemplatePath);
                    var emailBodyTemplate = mailService.ReadTextFile(templatePath);

                    var body = string.Format(
                        emailBodyTemplate,
                        unitHead.Nickname,
                        request.Period.Year,
                        param.StatusId == AnnualManpowerForecastRequestStatus.Verified ? "verifier" : "approver",
                        appSettings.Domain);

                    var mailModel = new MailModel
                    {
                        MailTo = unitHead.Email,
                        Subject = string.Empty,
                        Body = body
                    };

                    await mailService.SendMailAsync(mailModel);
                }
            }
        }

        private async Task<ManpowerFlow> GetUnitHeadAsync(string unitName)
        {
            var criteria = new GetFlowByUnitCriteria
            {
                UnitName = unitName
            };
            var flows = await baseService.GetDataAsync<ManpowerFlow>(criteria);
            return flows.FirstOrDefault(x => x.ProcessId == PeriodProcess.UnitHead);
        }

        private bool HasEditAddition(AnnualManpowerForecastRequest request, UpdateAnnualManpowerForecastRequestParam param)
        {
            return ((request.Details.Any(x => (x.LocalAddition > 0 || x.ExpatAddition > 0) && (param.Details == null || !param.Details.Any())))
                || (request.Details.Any(x => param.Details.Any(y => y.LocalAddition != x.LocalAddition || y.ExpatAddition != x.ExpatAddition))));
        }
    }
}