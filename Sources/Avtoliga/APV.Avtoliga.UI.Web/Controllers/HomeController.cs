using System;
using System.Text;
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
        public ActionResult Index(string type = null)
        {
            MainModel model = MainController.GetModel(type);
            return View("Index", model);
        }

        [AllowAnonymous]
        public ActionResult Articles(string groupId = null)
        {
            ArticlesModel model = ArticleController.FindArticles(groupId);
            return View("Articles", model);
        }

        [AllowAnonymous]
        public ActionResult Article(string groupId, string articleId)
        {
            ArticleInfo article = ArticleController.FindArticle(articleId);
            if (article != null)
            {
                return View("Article", article);
            }
            return RedirectToAction("Articles");
        }

        [AllowAnonymous]
        public ActionResult Requisites()
        {
            var model = new RequisitesModel();
            return View("Requisites", model);
        }

        [AllowAnonymous]
        public ActionResult Offers()
        {
            var model = new OffersModel();
            return View("Offers", model);
        }

        [AllowAnonymous]
        public ActionResult News()
        {
            var model = new NewsModel(false);
            return View("News", model);
        }

        [AllowAnonymous]
        public ActionResult NewsArchive()
        {
            var model = new NewsModel(true);
            return View("News", model);
        }

        [AllowAnonymous]
        public ActionResult Feedback()
        {
            var model = new FeedbackModel(false);
            return View("Feedback", model);
        }

        [AllowAnonymous]
        public ActionResult Feedbacks()
        {
            var model = new FeedbackModel(false);
            return View("Feedbacks", model);
        }

        [AllowAnonymous]
        public ActionResult FeedbacksArchive()
        {
            var model = new FeedbackModel(true);
            return View("Feedbacks", model);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            var model = new AboutModel();
            return View("About", model);
        }

        [AllowAnonymous]
        public ActionResult Contacts()
        {
            return View("Contacts");
        }

        [AllowAnonymous]
        public ActionResult Producers()
        {
            ProducerInfo[] producers = SearchController.GetAllProducers();
            return View("Producers", producers);
        }

        [AllowAnonymous]
        public ActionResult Producer(string producerId)
        {
            ProducerInfo producer = SearchController.FindProducer(producerId);
            if (producer != null)
            {
                return View("Producer", producer);
            }
            return RedirectToAction("Catalog");
        }

        [AllowAnonymous]
        public ActionResult Catalog()
        {
            TrademarkInfo[] trademarks = TrademarkController.GetAll();
            return View("Catalog", trademarks);
        }

        [AllowAnonymous]
        public ActionResult Trademark(string trademarkId)
        {
            TrademarkInfo trademark = TrademarkController.Find(trademarkId);
            if (trademark != null)
            {
                return View("Trademark", trademark);
            }
            return RedirectToAction("Catalog");
        }

        [AllowAnonymous]
        public ActionResult Product(string trademarkId, string modelId, string productId)
        {
            ProductInfo product = SearchController.FindProduct(trademarkId, modelId, productId);
            if (product != null)
            {
                return View("Product", product);
            }
            return RedirectToAction("Catalog");
        }

        [AllowAnonymous]
        public ActionResult Search(string trademarkId, string modelId, string group, string producerId = null)
        {
            SearchModel model = SearchController.Search(trademarkId, modelId, group, producerId);
            return View("Search", model);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        public ActionResult Price()
        {
            byte[] data = PriceController.GetPriceContent();
            //FileContentResult result = File(data, "application/vnd.ms-excel", "price");
            FileContentResult result = File(data, "text/csv", "price.csv");
            string newEtag = Guid.NewGuid().ToString();
            Response.ContentEncoding = Encoding.Unicode;
            Response.AddHeader("ETag", newEtag);
            Response.AddHeader("Cache-Control", "no-cache");
            Response.StatusCode = 200;
            return result;
        }
    }
}