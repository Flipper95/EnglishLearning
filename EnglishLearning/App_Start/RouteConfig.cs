using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EnglishLearning
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(name: "Lection",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Lection", action = "ShowLection" },
                constraints: new { id = @"\d" },
                namespaces: new[] { "EnglishLearning.Controllers" });

            //routes.MapRoute(name:"LectionChoice",
            //    url: "{controller}/{action}/{complexity}",
            //    defaults: new { controller = "Lection", action = "ShowByComplexity" },
            //    namespaces: new[] { "EnglishLearning.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
