using System;

namespace Metsys.WebOp.Mvc.Extensions
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Web.Mvc;

    public static class HtmlExtensions
    {
        private static readonly Configuration _configuration = Configuration.Instance;           

        public static string IncludeCss(this HtmlHelper html, string name)
        {
            if (_configuration.SmartDebug)
            {
                var output = SmartOutput(string.Format("{0}/{1}.css", _configuration.StylesFolder, name), IncludeCss);
                if (output != null) { return output; }
            }
            return IncludeCss(GetAssetPath(string.Format("/{0}/{1}.css", _configuration.StylesFolder, name)));            
        }
        private static string IncludeCss(string file)
        {
            return string.Format("<link href=\"{0}{1}\" rel=\"stylesheet\" type=\"text/css\">", _configuration.RootAssetPath, file);
        }
        public static string IncludeJs(this HtmlHelper html, string name)
        {
            if (_configuration.SmartDebug)
            {
                var output = SmartOutput(string.Format("{0}/{1}.js", _configuration.ScriptsFolder, name), IncludeJs);
                if (output != null) { return output; }
            }
            return IncludeJs(GetAssetPath(string.Format("/{0}/{1}.js", _configuration.ScriptsFolder, name)));            
        }
        private static string IncludeJs(string file)
        {
            return string.Format("<script src=\"{0}{1}\" type=\"text/javascript\"></script>", _configuration.RootAssetPath, file);
        }

        public static string Image(this HtmlHelper html, string name, int width, int height, string alt)
        {
            var path = GetAssetPath(string.Format("/{0}/{1}", _configuration.ImagesFolder, name));
            return string.Format("<img src=\"{0}{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" />", _configuration.RootAssetPath, path, width, height, alt);
        }
        public static string Image(this HtmlHelper html, string name, int width, int height, string alt, object properties)
        {
            var path = GetAssetPath(string.Format("/{0}/{1}", _configuration.ImagesFolder, name));
            var sb = new StringBuilder(100);
            sb.AppendFormat("<img src=\"{0}{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" ", _configuration.RootAssetPath, path, width, height, alt);
            foreach (var property in ToDictionary(properties))
            {
                sb.AppendFormat("{0}=\"{1}\" ", property.Key, html.Encode(property.Value));
            }
            sb.Append("/>");
            return sb.ToString();
        }
        public static string ImageOver(this HtmlHelper html, string name, int width, int height, string alt)
        {
            var path = GetAssetPath(string.Format("/{0}/{1}", _configuration.ImagesFolder, name));
            var rel = string.Empty;
            var overPath = path.Replace(".", "_o.");
            var assetsHashes = _configuration.AssetHashes;
            if (assetsHashes.ContainsKey(overPath))
            {
                rel = string.Format(" rel=\"{0}\"", assetsHashes[overPath]);
            }
            return string.Format("<img src=\"{0}{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" class=\"ro\"{5} />", _configuration.RootAssetPath, path, width, height, alt, rel);
        }

        private static string GetAssetPath(string name)
        {
            string hash;
            return !_configuration.AssetHashes.TryGetValue(name, out hash) ? name : string.Concat(name, '?', hash);
        }
        private static string SmartOutput(string name, Func<string, string> generator)
        {
            var files = _configuration.GetCombinedFrom(name);
            if (files == null || files.Length == 0) { return null; }

            var sb = new StringBuilder();
            foreach(var file in files)
            {
                sb.Append(generator(file));
            }
            return sb.ToString();
        }
                
        private static IEnumerable<KeyValuePair<string, object>> ToDictionary(object @object)
        {
             var properties = TypeDescriptor.GetProperties(@object);
             var hash = new Dictionary<string, object>(properties.Count);
             foreach (PropertyDescriptor descriptor in properties)
             {
                hash.Add(descriptor.Name, descriptor.GetValue(@object));
             }
             return hash;
        }
    }
}