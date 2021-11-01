namespace HRM.UserControl.Core.Extensions
{
    using Fanex.Data.Repository;
    using HRM.UserControl.Core.Repository;
    using HRM.UserControl.Core.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class DI
    {
        public static void InjectCoreDI(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<ICryptoService, CryptoService>();
        }
    }
}