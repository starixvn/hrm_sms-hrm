namespace Fanex.NexHR.Core
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Repositories;
    using Fanex.NexHR.Core.Services;
    using Unity;

    public static class CoreDIExtensions
    {
        public static void ResolveCoreDI(this IUnityContainer container)
        {
            container.RegisterType<IDynamicRepository, DynamicRepository>();
            container.RegisterType<IBaseRepository, BaseRepository>();
            container.RegisterType<IBaseService, BaseService>();
            container.RegisterType<ISessionService, SessionService>();
        }
    }
}