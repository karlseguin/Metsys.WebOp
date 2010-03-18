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
        IConfiguration AssetHashesFilePathIs(string path);
        IConfiguration EnableSmartDebug(string path);
    }
    
    public class Configuration : IConfiguration
    {
        
        private const string _assetHashesCacheKey = "Metsys.WebOp.Mvc.AssetHashes";
        private static readonly Configuration _instance = new Configuration();
        internal static Configuration Instance
        {
            get { return _instance; }
        }

        private string _rootAssetPath;
        private string _stylesFolder = "styles";
        private string _scriptsFolder = "js";
        private string _imagesFolder = "images";
        private string _assetHashesFilePath;
        private bool _enableSmartDebug;
        private IDictionary<string, string[]> _combinedData;
        
        internal string RootAssetPath
        {
            get { return _rootAssetPath; }
        }
        internal string StylesFolder
        {
            get { return _stylesFolder; }
        }
        internal string ScriptsFolder
        {
            get { return _scriptsFolder; }
        }
        internal string ImagesFolder
        {
            get { return _imagesFolder; }
        }
        internal string AssetFilePath
        {
            get { return _assetHashesFilePath; }
        }
        internal bool SmartDebug
        {
            get { return _enableSmartDebug; }
        }
        internal IDictionary<string, string> AssetHashes
        {
            get
            {
                return (IDictionary<string, string>)HttpRuntime.Cache[_assetHashesCacheKey] ?? ParseAndCacheAssetHashes();                
            }
        }
        internal string[] GetCombinedFrom(string path)
        {            
            if (_combinedData == null || !_combinedData.ContainsKey(path))
            {
                return null;          
            }
            return _combinedData[path];
        }

        private Configuration(){}
        
        public IConfiguration RootAssetPathIs(string path)
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            _rootAssetPath = path;
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
        public IConfiguration AssetHashesFilePathIs(string path)
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            _assetHashesFilePath = path;            
            return this;            
        }
        public IConfiguration EnableSmartDebug(string path)
        {
            _enableSmartDebug = true;            
            LoadCombinedData(path);
            return this;
        }

        private IDictionary<string, string> ParseAndCacheAssetHashes()
        {
            if (_assetHashesFilePath == null)
            {
                return new Dictionary<string, string>(0);
            }           
            var hashes = new Dictionary<string, string>(20, StringComparer.InvariantCultureIgnoreCase);
            using (var sr = new StreamReader(_assetHashesFilePath))
            {
                while (sr.Peek() >= 0)
                {
                    var parts = sr.ReadLine().Split('|');
                    hashes.Add(parts[0], parts[1]);
                }
            }
            HttpRuntime.Cache.Insert(_assetHashesCacheKey, hashes, new CacheDependency(_assetHashesFilePath));
            return hashes;
        }

        private void LoadCombinedData(string path)
        {
            _combinedData = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);            
            foreach(var line in File.ReadAllLines(path))
            {
                if (!line.Trim().ToLower().StartsWith("combine")) { continue; }

                var parts = line.Split(new[]{':'}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3){ continue; }

                var key = parts[1].Trim().Replace("\\", "/");
                var files = parts[2].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < files.Length; ++i )
                {
                    files[i] = string.Concat('/', files[i].Trim().Replace("\\", "/"));
                }
                _combinedData[key] = files;
            }
        }
    }
}