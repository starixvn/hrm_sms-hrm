namespace GO.Letstalk.WebAPI.Extentions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using GO.Letstalk.WebAPI.Models;
    using GO.Letstalk.WebAPI.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DI
    {
        public static void RegisterDI(this IServiceCollection services, IConfiguration configuration)
        {
            var webhookConfigs = configuration.GetSection("WebhookConfigurations").Get<List<WebhookConfiguration>>();
            services.AddSingleton(webhookConfigs);

            var botConfigs = configuration.GetSection("LetstalkBotConfigurations").Get<List<BotConfig>>();
            services.AddSingleton(botConfigs);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddSingleton<ICryptographyService, CryptographyService>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddTransient<HttpClientService>();
            services.AddTransient<LetstalkService>();

            services.AddSingleton<Func<int, ILetstalkService>>(serviceProvider => botId =>
            {
                var botConfig = botConfigs.FirstOrDefault(config => config.BotAccountId == botId);

                if (botConfig == null)
                {
                    throw new ArgumentNullException($"The bot ID: {botId} does not existed in config file");
                }

                var httpClientService = new HttpClientService(
                    serviceProvider.GetRequiredService<IHttpClientFactory>(),
                    serviceProvider.GetService<IJwtService>(), botConfig);

                return new LetstalkService(
                    serviceProvider.GetRequiredService<ICryptographyService>(),
                    httpClientService,
                    botConfig);
            });
        }
    }
}