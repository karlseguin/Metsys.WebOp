namespace Metsys.WebOp.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Caching;

    public interface IInitialConfiguration
    {
        IConfiguration RootAssetPathIs(string relative, string full);
    }
    public interface IConfiguration
    {        
        IConfiguration StylesAreIn(string folderName);
        IConfiguration ScriptsAreIn(string folderName);
        IConfiguration ImagesAreIn(string folderName);
        IConfiguration AssetHashesFileIs(string file);
        IConfiguration EnableSmartDebug();
        IConfiguration EnableSmartDebug(bool enabled);
    }

    public class Configuration : IInitialConfiguration, IConfiguration
    {        
        private const string _assetHashesCacheKey = "Metsys.WebOp.Mvc.AssetHashes";
        private static readonly Configuration _instance = new Configuration();
        internal static Configuration Instance
        {
            get { return _instance; }
        }

        private string _rootAssetPath;
        private string _fullAssetPath;
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
        internal string FullAssetPath
        {
            get { return _fullAssetPath; }
        }
        internal bool SmartDebug
        {
            get { return _enableSmartDebug; }
        }
        internal string CommandFile
        {
            get { return string.Concat(_fullAssetPath, "\\webop.dat"); }
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

        public IConfiguration RootAssetPathIs(string relative, string full)
        {
            if (relative == null) { throw new ArgumentNullException("path"); }
            if (full == null) { throw new ArgumentNullException("full"); }
            _rootAssetPath = relative.EndsWith("/") ? relative.TrimEnd('/') : relative;
            _fullAssetPath = full;
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
        public IConfiguration AssetHashesFileIs(string file)
        {
            if (file == null) { throw new ArgumentNullException("file"); }
            _assetHashesFilePath = string.Concat(_fullAssetPath, '\\', file);            
            return this;            
        }

        public IConfiguration EnableSmartDebug()
        {
            return EnableSmartDebug(true);
        }

        public IConfiguration EnableSmartDebug(bool enabled)
        {
            _enableSmartDebug = enabled;
            _combinedData = LoadCombinedData();
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

        private static IDictionary<string, string[]> LoadCombinedData()
        {
            var data = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            ParseCommandsLookingFor("combine", line =>
               {
                   var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                   if (parts.Length != 3) { return; }

                   var key = parts[1].Trim().Replace("\\", "/");
                   var files = parts[2].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                   for (var i = 0; i < files.Length; ++i)
                   {
                       files[i] = string.Concat('/', files[i].Trim().Replace("\\", "/"));
                   }
                   data[key] = files;
               });
            return data;

        }

        public static IDictionary<string, string> LoadZippedLookup()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            ParseCommandsLookingFor("zip", line =>
            {
                var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) { return; }

                var files = parts[1].Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);

                foreach(var file in files)
                {
                    var target = string.Concat(Instance._rootAssetPath, '\\', file.Trim());
                    data[target.Replace('\\', '/')] = string.Concat(target, ".gz");
                }                              
            });
            return data;
        }

        private static void ParseCommandsLookingFor(string commandName, Action<string> action)
        {
            var lines = new List<string>();
            var isInCommand = false;
            foreach (var line in File.ReadAllLines(Instance.CommandFile))
            {
                var l = line.Trim().ToLower();
                if (l.StartsWith(commandName))
                {
                    lines.Add(l);
                    isInCommand = true;                    
                } 
                else if (isInCommand && l.StartsWith("-"))
                {
                    lines[lines.Count - 1] += "," + l.Substring(1);
                }
                else
                {
                    isInCommand = false;
                }
            }
            foreach(var line in lines)
            {
                action(line);
            }
        }
    }
}