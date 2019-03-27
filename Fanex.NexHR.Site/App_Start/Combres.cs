[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Fanex.NexHR.Site.App_Start.Combres), "PreStart")]
namespace Fanex.NexHR.Site.App_Start
{
    using System.Web.Routing;
    using global::Combres;

    public static class Combres
    {
        public static void PreStart()
        {
            RouteTable.Routes.AddCombresRoute("Combres");
        }
    }
}