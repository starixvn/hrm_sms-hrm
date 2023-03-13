namespace HRM.Letstalk.Client.Extentions
{
    using HRM.Letstalk.Client.Models;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using static HRM.Letstalk.Client.Services.LogService;
    using Microsoft.Extensions.Configuration;
    using HRM.Letstalk.Client.Services.Interface;
    using System;
    using System.Linq;
    using System.Net.Http;
    using HRM.Letstalk.Client.Services;
    using HRM.Letstalk.Client.Constants;
    using System.Net;

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

            RegisterHttpClientFactory(services);
            RegisterLetstalkBotService(services, botConfigs);

        }

        private static void RegisterHttpClientFactory(IServiceCollection services)
        {
            services.AddHttpClient(HttpClientName.DefaultHttpClientWithProxy);
        }

        private static void RegisterLetstalkBotService(this IServiceCollection services, List<BotConfig> botConfigs)
        {
            services.AddSingleton<ICryptographyService, CryptographyService>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddTransient<HttpClientService>();
            services.AddTransient<LetstalkService>();
            services.AddTransient<Func<int, ILetstalkService>>(serviceProvider => botId =>
            {
                var botConfig = botConfigs.FirstOrDefault(config => config.BotAccountId == botId);

                if (botConfig == null)
                {
                    throw new ArgumentNullException($"The bot ID: {botId} does not existed in config file");
                }

                var httpClientService = new HttpClientService(serviceProvider.GetRequiredService<IHttpClientFactory>(), serviceProvider.GetService<IJwtService>(), botConfig);

                return new LetstalkService(serviceProvider.GetRequiredService<ICryptographyService>(), httpClientService, botConfig);
            });
        }
    }
}
