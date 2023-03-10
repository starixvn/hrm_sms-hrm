using HRM.Letstalk.Client.Models;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using static HRM.Letstalk.Client.Services.LogService;
using Microsoft.Extensions.Configuration;

namespace HRM.Letstalk.Client.Extentions
{
    public static class DI
    {
        public static void ResolveCoreDI(this IServiceCollection services, IConfiguration configuration)
        {
            var webhookConfigurationSection = configuration.GetSection("WebhookConfigurations");
            services.Configure<List<WebhookConfiguration>>(options => webhookConfigurationSection.Bind(options));
            var webhookConfigs = webhookConfigurationSection.Get<List<WebhookConfiguration>>();
            services.AddSingleton(webhookConfigs);

            var botConfigSections = configuration.GetSection("LetstalkBotConfigurations");
            services.Configure<List<BotConfig>>(options => botConfigSections.Bind(options));
            var botConfigs = botConfigSections.Get<List<BotConfig>>();
            services.AddSingleton(botConfigs);

            services.AddSingleton<ILetstalkLogService, LetstalkLogService>();

        }
    }
}
