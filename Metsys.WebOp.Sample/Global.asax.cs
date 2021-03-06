﻿namespace Metsys.WebOp.Sample
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
                //first argument could be something like http://cdn.mysite.com/assets/     
                c => c.RootAssetPathIs("/assets/", Server.MapPath("/assets/"))
                        
                        .AssetHashesFileIs("hashes.dat")
                        .StylesAreIn("css")
#if DEBUG
                        .EnableSmartDebug()
#endif                        
            );
        }
    }
}