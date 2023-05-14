using System.Web.Mvc;
using System.Web.Routing;

namespace CAP_TEAM05_2022
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "categories",
                "quan-ly-danh-muc",
                new { controller = "categories", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "sale_details", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
