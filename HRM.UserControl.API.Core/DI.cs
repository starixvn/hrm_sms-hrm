namespace HRM.UserControl.API.Core
{
    using HRM.UserControl.API.Core.Services;
    using HRM.UserControl.API.Core.Services.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class DI
    {
        public static void RegisterDI(this IServiceCollection services)
        {
            services.AddTransient<IJwtManagerService, JwtManagerService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISiteService, SiteService>();
        }
    }
}