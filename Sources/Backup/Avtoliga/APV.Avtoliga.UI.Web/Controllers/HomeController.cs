using System.Web.Mvc;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult About()
        {
            return View("About");
        }

        public ActionResult Contacts()
        {
            return View("Contacts");
        }

        public ActionResult Producers()
        {
            ProducerInfo[] producers = SearchController.GetAllProducers();
            return View("Producers", producers);
        }

        public ActionResult Producer(string producerId)
        {
            ProducerInfo producer = SearchController.FindProducer(producerId);
            if (producer != null)
            {
                return View("Producer", producer);
            }
            return RedirectToAction("Catalog");
        }

        public ActionResult Catalog()
        {
            TrademarkInfo[] trademarks = TrademarkController.GetAll();
            return View("Catalog", trademarks);
        }

        public ActionResult Trademark(string trademarkId)
        {
            TrademarkInfo trademark = TrademarkController.Find(trademarkId);
            if (trademark != null)
            {
                return View("Trademark", trademark);
            }
            return RedirectToAction("Catalog");
        }

        public ActionResult Product(string trademarkId, string modelId, string productId)
        {
            ProductInfo product = SearchController.FindProduct(trademarkId, modelId, productId);
            if (product != null)
            {
                return View("Product", product);
            }
            return RedirectToAction("Catalog");
        }

        public ActionResult Search(string trademarkId, string modelId, string group, string producerId = null)
        {
            SearchModel model = SearchController.Search(trademarkId, modelId, group, producerId);
            return View("Search", model);
        }

        //[OutputCache(Duration = int.MaxValue, Location = OutputCacheLocation.Client)]
        public ActionResult Image(string imageId)
        {
            //http://habrahabr.ru/post/204464/
            // https://developers.google.com/web/fundamentals/performance/optimizing-content-efficiency/http-caching
            ImageEntity image = ImageController.GetImage(imageId);
            string etag = Request.Headers["If-None-Match"] ?? string.Empty;
            string newEtag = image.Tag.ToString();
            Response.AddHeader("ETag", newEtag);
            Response.AddHeader("Cache-Control", "no-cache");
            //Response.AddHeader("Expires", "Fri, 01 Jul 2030 00:00:00 GMT");
            Response.AddHeader("Last-Modified", "Thu, 01 Jul 2000 00:00:00 GMT");
            bool notModified = (etag == newEtag);
            FileContentResult result = (notModified) ? File(new byte[0], "image/png") : File(image.Data, "image/png");
            Response.StatusCode = (notModified) ? 304 : 200;
            return result;
        }
    }
}