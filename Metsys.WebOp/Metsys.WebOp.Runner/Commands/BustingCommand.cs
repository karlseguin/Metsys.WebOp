namespace Metsys.WebOp.Runner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class BustingCommand : ICommand
    {
        private static readonly Regex _stylesheetPattern = new Regex(@"\('?(?<file>.*?\.(gif|jpg|png))(\?.*?)?'?\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture); 
        
        private readonly string _output;
        private static IDictionary<string, string> _hashes;
        
        public BustingCommand(string[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new Exception("Busting command should have 2 arguments");
            }
            _output = arguments[1].Trim();
        }
        
        public void Execute(string rootPath)
        {
            Console.WriteLine(string.Format("Generating hash file at {0} for all resources within {1}", _output, rootPath));
            _hashes = new Dictionary<string, string>();
            foreach(var directory in Directory.GetDirectories(rootPath))
            {
                HuntForResources(rootPath, directory.Remove(0, rootPath.Length));    
            }
            OutputFile(rootPath + _output);
            ProcessStyleSheets(rootPath);                      
        }

        private static void OutputFile(string output)
        {
            using (var sw = new StreamWriter(output))
            {
                foreach (var kvp in _hashes)
                {
                    sw.WriteLine("{0}|{1}", kvp.Key, kvp.Value);
                }
            }
        }

        private static void HuntForResources(string rootPath, string container)
        {
            foreach (var directory in Directory.GetDirectories(rootPath + container))
            {
                if ((new DirectoryInfo(directory).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    continue;
                }
                HuntForResources(rootPath, directory.Remove(0, rootPath.Length));
            }
            foreach (var file in Directory.GetFiles(rootPath + container))
            {
                var assetName = file.Remove(0, rootPath.Length - 1).Replace('\\', '/');
                _hashes.Add(assetName, CreateSignature(file));
            }
        }

        private static string CreateSignature(string file)
        {
            byte[] bytes;
            using (var hash = new Crc32())
            {
                bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(File.ReadAllText(file)));
            }

            var data = new StringBuilder();
            Array.ForEach(bytes, b => data.Append(b.ToString("x2")));
            return data.ToString();
        }

        private static void ProcessStyleSheets(string root)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {             
                ProcessStyleSheets(directory);
            }
            foreach (var file in Directory.GetFiles(root, "*.css"))
            {
                var content = File.ReadAllText(file);
                content = _stylesheetPattern.Replace(content, new MatchEvaluator(ApplyHashToMatchedStyle));
                File.WriteAllText(file, content);
            }
        }

        private static string ApplyHashToMatchedStyle(Match match)
        {
            var target = match.Groups[1].Value;
            foreach(var key in _hashes.Keys)
            {
                if (target.EndsWith(key))
                {
                    return string.Format("('{0}?{1}')", match.Groups[1].Value, _hashes[key]);                    
                }                
            }
            return match.Groups[0].Value;
        }
    }
}