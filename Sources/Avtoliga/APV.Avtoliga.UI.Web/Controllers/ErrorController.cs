using System.Web.Mvc;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var info = ViewData.Model as ErrorInfo;

            if ((info == null) || (info.NotFound))
            {
                Response.Redirect("~/");
                //return RedirectToAction("Index", "Home");
            }

            return View("Error", info);
        }
    }
}