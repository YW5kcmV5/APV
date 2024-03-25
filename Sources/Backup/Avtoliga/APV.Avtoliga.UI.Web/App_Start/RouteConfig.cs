using System.Web.Mvc;
using System.Web.Routing;

namespace APV.Avtoliga.UI.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //Main routes
            //routes.MapRoute("ImageById", "image/{imageId}", new { controller = "Home", action = "Image", imageId = UrlParameter.Optional });
            routes.MapRoute("ImageById", "image/{imageId}", new { controller = "Home", action = "Image" });
            //routes.MapRoute("ImageByName", "image/{imageId}.png", new { controller = "Home", action = "Image" });
            routes.MapRoute("Contacts", "contacts", new { controller = "Home", action = "Contacts" });
            routes.MapRoute("Producers", "producers", new { controller = "Home", action = "Producers" });
            routes.MapRoute("Catalog", "catalog", new { controller = "Home", action = "Catalog" });
            routes.MapRoute("Trademark", "catalog/{trademarkId}", new { controller = "Home", action = "Trademark" });
            routes.MapRoute("Product", "catalog/{trademarkId}/{modelId}/{productId}", new { controller = "Home", action = "Product" });
            routes.MapRoute("Producer", "producer/{producerId}", new { controller = "Home", action = "Producer" });
            routes.MapRoute("Search", "search/{trademarkId}/{modelId}/{group}", new { controller = "Home", action = "Search" });
            routes.MapRoute("SearchEmpty", "search", new { controller = "Home", action = "Search" });
            routes.MapRoute("SearchByProducer", "search/{trademarkId}/{modelId}/{group}/{producerId}", new { controller = "Home", action = "Search" });
            routes.MapRoute("About", "about", new { controller = "Home", action = "About" });
            //Default
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}