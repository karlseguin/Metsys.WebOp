namespace Metsys.WebOp.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Caching;

    public interface IConfiguration
    {
        IConfiguration RootAssetPathIs(string path);
        IConfiguration StylesAreIn(string folderName);
        IConfiguration ScriptsAreIn(string folderName);
        IConfiguration ImagesAreIn(string folderName);
        IConfiguration AssetFilePathIs(string path);
    }
    
    public class Configuration : IConfiguration
    {
        private const string _assetHashesCacheKey = "Metsys.WebOp.Mvc.AssetHashes";
        private string _rootAssetPath;
        private string _stylesFolder = "styles";
        private string _scriptsFolder = "js";
        private string _imagesFolder = "images";
        private string _assetFilePath;
        
        private static readonly Configuration _instance = new Configuration();            
        internal static Configuration Instance
        {
            get { return _instance;}
        }
        
        internal static string RootAssetPath
        {
            get { return _instance._rootAssetPath; }
        }
        internal static string StylesFolder
        {
            get { return _instance._stylesFolder; }
        }
        internal static string ScriptsFolder
        {
            get { return _instance._scriptsFolder; }
        }
        internal static string ImagesFolder
        {
            get { return _instance._imagesFolder; }
        }
        internal static IDictionary<string, string> AssetHashes
        {
            get
            {
                return (IDictionary<string, string>)HttpRuntime.Cache[_assetHashesCacheKey] ?? _instance.ParseAndCacheAssetHashes();                
            }
        }

        private Configuration(){}
        
        public IConfiguration RootAssetPathIs(string path)
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            _rootAssetPath = path.EndsWith("/") ? path : string.Concat(path, '/');            
            return this;
        }

        public IConfiguration StylesAreIn(string folderName)
        {
            if (folderName == null) { throw new ArgumentNullException("folderName"); }
            _stylesFolder = folderName;
            return this;
        }

        public IConfiguration ScriptsAreIn(string folderName)
        {
            if (folderName == null) { throw new ArgumentNullException("folderName"); }
            _scriptsFolder = folderName;
            return this;
        }

        public IConfiguration ImagesAreIn(string folderName)
        {
            if (folderName == null) { throw new ArgumentNullException("folderName"); }
            _imagesFolder = folderName;
            return this;
        }

        public IConfiguration AssetFilePathIs(string path)
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            _assetFilePath = path;            
            return this;            
        }

        private IDictionary<string, string> ParseAndCacheAssetHashes()
        {
            if (_assetFilePath == null)
            {
                return null;
            }           
            var hashes = new Dictionary<string, string>(20, StringComparer.InvariantCultureIgnoreCase);
            using (var sr = new StreamReader(_assetFilePath))
            {
                while (sr.Peek() >= 0)
                {
                    var parts = sr.ReadLine().Split('|');
                    hashes.Add(parts[0], parts[1]);
                }
            }
            HttpRuntime.Cache.Insert(_assetHashesCacheKey, hashes, new CacheDependency(_assetFilePath));
            return hashes;
        }
    }
}