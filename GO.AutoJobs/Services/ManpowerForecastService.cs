namespace GO.AutoJobs.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using GO.AutoJobs.Criteria.ManpowerForecast;
    using GO.AutoJobs.Models;
    using GO.AutoJobs.Models.ManpowerForecast;
    using GO.Core.Models;
    using GO.Core.Services;
    using Microsoft.AspNetCore.Hosting;

    public class ManpowerForecastService : IManpowerForecastService
    {
        private const string NotiStartToUnitHeadTemplatePath = "EmailTemplates/ManpowerForecast/NotiStartToUnitHead.txt";
        private const string NotiStartToVerifierTemplatePath = "EmailTemplates/ManpowerForecast/NotiStartToVerifier.txt";
        private const string NotiStartToApproverTemplatePath = "EmailTemplates/ManpowerForecast/NotiStartToApprover.txt";

        private readonly AppSettings appSettings;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly ILogger serilog;
        private readonly IBaseService baseService;
        private readonly IBaseMailService mailService;

        public ManpowerForecastService(
            AppSettings appSettings,
            IWebHostEnvironment hostEnvironment,
            ILogger serilog,
            IBaseService baseService,
            IBaseMailService mailService)
        {
            this.appSettings = appSettings;
            this.hostEnvironment = hostEnvironment;
            this.serilog = serilog;
            this.baseService = baseService;
            this.mailService = mailService;
        }

        public async Task SendNotificationAsync()
        {
            var criteria = new GetDataToSendNotificationCriteria();

            (var periods, var empSettings) = await baseService.FetchMultipleAsync<ManpowerForecastPeriod, PeriodEmployeeSetting>(criteria);
            var notiStartToUnitHeadTemplatePath = Path.Combine(hostEnvironment.ContentRootPath, NotiStartToUnitHeadTemplatePath);
            var notiStartToVerifierTemplatePath = Path.Combine(hostEnvironment.ContentRootPath, NotiStartToVerifierTemplatePath);
            var notiStartToApproverTemplatePath = Path.Combine(hostEnvironment.ContentRootPath, NotiStartToApproverTemplatePath);
            foreach(var period in periods)
            {
                var emps = empSettings.Where(x => x.PeriodId == period.PeriodId);

                foreach(var emp in emps)
                {
                    const byte VerifyProcess = 2;
                    const byte ApproveProcess = 3;

                    string emailBodyTemplate;
                    switch (emp.ProcessId)
                    {
                        case 1:
                            emailBodyTemplate = mailService.ReadTextFile(notiStartToUnitHeadTemplatePath);
                            await SendNotiStartToUnitHeadAsync(period, emp, emailBodyTemplate);
                            break;

                        case VerifyProcess:
                            emailBodyTemplate = mailService.ReadTextFile(notiStartToVerifierTemplatePath);
                            await SendNotiStartToUnitHeadAsync(period, emp, emailBodyTemplate);
                            break;

                        case ApproveProcess:
                            emailBodyTemplate = mailService.ReadTextFile(notiStartToApproverTemplatePath);
                            await SendNotiStartToUnitHeadAsync(period, emp, emailBodyTemplate);
                            break;

                        default:
                            // nothing
                            break;
                    }
                }
            }
        }

        private async Task SendNotiStartToUnitHeadAsync(ManpowerForecastPeriod period, PeriodEmployeeSetting setting, string emailBodyTemplate)
        {
            emailBodyTemplate = string.Format(
                emailBodyTemplate,
                setting.Nickname,
                period.Year,
                appSettings.Domain,
                period.Year - 1,
                GetFromDate(setting.FromDate),
                GetToDate(setting.ToDate));

            var mailModel = new MailModel
            {
                MailTo = setting.Email,
                Subject = "",
                Body = emailBodyTemplate
            };

            try
            {
                await mailService.SendMailAsync(mailModel);
            }
            catch (Exception ex)
            {
                await serilog.ErrorAsync(ex.Message, ex);
            }
        }

        private static string GetFromDate(DateTime fromDate) => string.Format("{0} 00:00", fromDate.ToString("dd MMM, yyyy"));

        private static string GetToDate(DateTime toDate) => string.Format("{0} 23:59", toDate.ToString("dd MMM, yyyy"));
    }
}