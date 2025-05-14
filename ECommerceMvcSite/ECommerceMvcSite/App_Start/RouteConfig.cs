using System.Web.Mvc;
using System.Web.Routing;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        // Admin için özel rota
        routes.MapRoute(
            name: "Admin", // Rota ismi
            url: "Admin/{action}/{id}", // AdminController'ın route'u
            defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
        );

        // Varsayılan rota
        routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
        );
    }
}
