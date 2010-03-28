namespace Metsys.WebOp.Mvc
{
    using System;
    using System.Web;
    using System.Collections.Generic;
    using System.IO;

    public class WebOpZipHttpModule : IHttpModule
    {
        private static readonly IDictionary<string, string> _contentTypeLookup = new Dictionary<string, string>
        {
            {".css", "text/css"},
            {".js", "application/x-javascript"},
        };

        private static IDictionary<string, string> _zippedFiles;

        public void Init(HttpApplication context)
        {
            _zippedFiles = Configuration.LoadZippedLookup();      
            context.BeginRequest += OnBeginRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication) sender;
            var request = application.Request;
            var url = request.Url.AbsolutePath;
            var extension = Path.GetExtension(url);
            if (request.Headers["Accept-Encoding"].Contains("gzip") && _contentTypeLookup.ContainsKey(extension) && IsZipped(url))
            {
                var response = application.Response;
                response.AddHeader("Content-Encoding", "gzip");
                response.AddHeader("Content-Type", _contentTypeLookup[extension]);
                response.WriteFile(_zippedFiles[url]);
                application.CompleteRequest();
            }

        }

        private static bool IsZipped(string url)
        {
            return _zippedFiles.ContainsKey(url);
        }
        public void Dispose(){ }
    }
}