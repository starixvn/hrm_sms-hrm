[assembly: WebActivator.PreApplicationStartMethod(typeof(Fanex.NexHR.Site.AppStart.Combres), "PreStart")]
namespace Fanex.NexHR.Site.AppStart
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