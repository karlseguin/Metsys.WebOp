namespace Metsys.WebOp.Mvc
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Web.Mvc;

    public static class HtmlExtensions
    {
        public static string IncludeCss(this HtmlHelper html, string name)
        {
            var path = GetAssetPath(string.Format("{0}/{1}.css", Configuration.StylesFolder, name));
            return string.Format("<link href=\"{0}{1}\" rel=\"stylesheet\" type=\"text/css\">", Configuration.RootAssetPath, path);
        }
        public static string IncludeJs(this HtmlHelper html, string name)
        {
            var path = GetAssetPath(string.Format("{0}/{1}.js", Configuration.ScriptsFolder, name));
            return string.Format("<script src=\"{0}{1}\" type=\"text/javascript\"></script>", Configuration.RootAssetPath, path);
        }

        public static string Image(this HtmlHelper html, string name, int width, int height, string alt)
        {
            var path = GetAssetPath(string.Format("{0}/{1}", Configuration.ImagesFolder, name));
            return string.Format("<img src=\"{0}{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" />", Configuration.RootAssetPath, path, width, height, alt);
        }
        public static string Image(this HtmlHelper html, string name, int width, int height, string alt, object properties)
        {
            var path = GetAssetPath(string.Format("{0}/{1}", Configuration.ImagesFolder, name));
            var sb = new StringBuilder(100);
            sb.AppendFormat("<img src=\"{0}{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" ", Configuration.RootAssetPath, path, width, height, alt);
            foreach (var property in ToDictionary(properties))
            {
                sb.AppendFormat("{0}=\"{1}\" ", property.Key, html.Encode(property.Value));
            }
            sb.Append("/>");
            return sb.ToString();
        }
        public static string ImageOver(this HtmlHelper html, string name, int width, int height, string alt)
        {
            var path = GetAssetPath(string.Format("{0}/{1}", Configuration.ImagesFolder, name));
            var rel = string.Empty;
            var overPath = path.Replace(".", "_o.");
            var assetsHashes = Configuration.AssetHashes;
            if (assetsHashes.ContainsKey(overPath))
            {
                rel = string.Format(" rel=\"{0}\"", assetsHashes[overPath]);
            }
            return string.Format("<img src=\"{0}{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" class=\"ro\"{5} />", Configuration.RootAssetPath, path, width, height, alt, rel);
        }

        private static string GetAssetPath(string name)
        {
            string hash;
            return !Configuration.AssetHashes.TryGetValue(name, out hash) ? name : string.Concat(name, '?', hash);
        }
                
        private static IDictionary<string, object> ToDictionary(object @object)
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