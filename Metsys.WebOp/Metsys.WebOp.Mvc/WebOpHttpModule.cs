namespace Metsys.WebOp.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    
    public class WebOpHttpModule : IHttpModule
    {
        private static readonly List<string> _headersToRemove = new List<string> { "X-AspNet-Version", "X-AspNetMvc-Version", "Etag", "Server",  };
        private static readonly List<string> _longCacheExtensions = new List<string> { ".js", ".css", ".png", ".jpg",".gif", };

        public void Init(HttpApplication context)
        {
            context.EndRequest += OnRequestEnd;
        }
        public void Dispose(){}

        private void OnRequestEnd(object sender, EventArgs e)
        {    
#if !DEBUG  
            //doesn't work great in Cassini + you don't want caching on while developing.
            //HttpContext.Current can be null within OnPreSendRequestHeaders, so we do this here
            var context = HttpContext.Current;            
            _headersToRemove.ForEach(h => context.Response.Headers.Remove(h));                        
            var extension = Path.GetExtension(context.Request.Url.AbsolutePath); 
            if (_longCacheExtensions.Contains(extension))
            {
                context.Response.CacheControl = "Public";
                context.Response.Expires = 44000; //slightly over 1 month
            }
#endif
        }
    }
}