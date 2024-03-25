using System.Web.Optimization;

namespace APV.Avtoliga.UI.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Java-script:

            //jQuery v2.1.4
            bundles.Add(
                new ScriptBundle("~/bundles/jquery")
                    .Include("~/Content/Scripts/jQuery/jquery-2.1.4.min.js")
                );

            //AngularJS v1.0.6
            bundles.Add(
                new ScriptBundle("~/bundles/angularjs")
                    .Include("~/Content/Scripts/Angular/angular.min.js")
                );

            
            bundles.Add(
                new ScriptBundle("~/bundles/cufon-yui")
                );

            //Plugins
            bundles.Add(
                new ScriptBundle("~/bundles/plugins")
                    //Cufon 1.09i
                    .Include("~/Content/Scripts/Cufon/cufon-yui.js")
                    .Include("~/Content/Scripts/Cufon/fonts.js")
                    //Magnific Popup - v1.0.0 - 2014-12-12
                    .Include("~/Content/Scripts/MagnificPopup/jquery.magnific-popup.min.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/avtoliga")
                            .Include("~/Content/Scripts/Extensions.js")
                            .Include("~/Content/Scripts/AvtoligaApp.js")
                            .Include("~/Content/Scripts/HeaderController.js")
                            .Include("~/Content/Scripts/GalleryController.js")
                            .Include("~/Content/Scripts/ProductController.js")
                            .Include("~/Content/Scripts/TrademarkController.js")
                            .Include("~/Content/Scripts/SearchMenuController.js")
                            .Include("~/Content/Scripts/NewsController.js")
                            .Include("~/Content/Scripts/FeedbacksController.js")
                            .Include("~/Content/Scripts/FeedbackController.js")
                );

            //CSS:
            bundles.Add(new StyleBundle("~/bundles/css")
                            .Include("~/Content/Css/Main.css")
                            .Include("~/Content/Css/Dialogs.css")
                            .Include("~/Content/Css/Cufon.css")
                            .Include("~/Content/Css/Lightbox.css")
                            .Include("~/Content/Css/MagnificPopup/magnific-popup.css")
                );

            //Config:
            //BundleTable.EnableOptimizations = true;
            BundleTable.EnableOptimizations = false;
        }
    }
}