using System.Web.Mvc;
using System.Web.Routing;

namespace APV.Avtoliga.UI.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Clear();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //Main routes
            //routes.MapRoute("ImageById", "image/{imageId}", new { controller = "Home", action = "Image", imageId = UrlParameter.Optional });
            routes.MapRoute("ImageById", "image/{imageId}", new { controller = "Home", action = "Image" });
            //routes.MapRoute("ImageByName", "image/{imageId}.png", new { controller = "Home", action = "Image" });
            routes.MapRoute("Price", "price", new { controller = "Home", action = "Price" });
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
            routes.MapRoute("News", "news", new { controller = "Home", action = "News" });
            routes.MapRoute("NewsArchive", "news/archive", new { controller = "Home", action = "NewsArchive" });
            routes.MapRoute("Feedback", "feedback", new { controller = "Home", action = "Feedback" });
            routes.MapRoute("Feedbacks", "feedbacks", new { controller = "Home", action = "Feedbacks" });
            routes.MapRoute("FeedbacksArchive", "feedbacks/archive", new { controller = "Home", action = "FeedbacksArchive" });
            routes.MapRoute("Offers", "offers", new { controller = "Home", action = "Offers" });
            routes.MapRoute("Requisites", "requisites", new { controller = "Home", action = "Requisites" });
            routes.MapRoute("Article", "articles/{groupId}/{articleId}", new { controller = "Home", action = "Article" });
            routes.MapRoute("Articles", "articles/{groupId}", new { controller = "Home", action = "Articles", groupId = UrlParameter.Optional });
            //Should be last
            routes.MapRoute("Index", "{type}", new { controller = "Home", action = "Index", type = UrlParameter.Optional });
            //API
            routes.MapRoute("ApiNewsToggle", "api/news_toggle", new { controller = "Service", action = "NewsToggle" });
            routes.MapRoute("ApiFeedbackToggle", "api/feedback_toggle", new { controller = "Service", action = "FeedbackToggle" });
            routes.MapRoute("ApiFeedback", "api/feedback", new { controller = "Service", action = "Feedback" });
            routes.MapRoute("Login", "api/login", new { controller = "Service", action = "Login" });
            routes.MapRoute("Logout", "api/logout", new { controller = "Service", action = "Logout" });
            //Default
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}