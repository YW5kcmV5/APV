using System.Web.Mvc;
using APV.Avtoliga.Core.Application;
using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Controllers
{
    public class ServiceController : Controller//ApiController
    {
        [HttpPost]
        [RequireHttps]
        public ActionResult Login(string username, string password)
        {
            bool logged = ContextManager.Login(username, password);
            return Json(logged, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void Logout()
        {
            ContextManager.Logout();
        }

        [HttpGet]
        public ActionResult NewsToggle(int id, bool liked)
        {
            int likes = NewsController.SetLike(id, liked);
            return Json(likes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FeedbackToggle(int id, bool liked)
        {
            int likes = FeedbackController.SetLike(id, liked);
            return Json(likes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Feedback(FeedbackInfo feedback)
        {
            ApiResult result = FeedbackController.SaveFeedback(feedback);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}