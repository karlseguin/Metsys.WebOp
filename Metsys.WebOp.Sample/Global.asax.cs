namespace Metsys.WebOp.Sample
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Default", "{controller}/{action}/{id}", new {controller = "Home", action = "Index", id = UrlParameter.Optional});
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            
            Mvc.WebOpConfiguration.Initialize
            (
                c => c.RootAssetPathIs("/assets/")
                        .CommandFilePathIs(Server.MapPath("~/assets/webop.dat"))
                        .AssetHashesFilePathIs(Server.MapPath("~/assets/hashes.dat"))
                        .StylesAreIn("css")
#if DEBUG
                        .EnableSmartDebug()
#endif                        
            );
        }
    }
}