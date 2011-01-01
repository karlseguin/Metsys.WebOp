namespace Metsys.WebOp.Runner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Yahoo.Yui.Compressor;

    public class ShrinkCommand: ICommand
    {
        private static readonly IDictionary<string, Func<string, string>> _compressors = new Dictionary<string, Func<string, string>>
        {
            {".js", input => JavaScriptCompressor.Compress(input)},
            {".css", input => CssCompressor.Compress(input)},
        };

        private readonly IList<string> _inputs;

        public ShrinkCommand(IList<string> arguments)
        {
            if (arguments.Count < 1)
            {
                throw new Exception("Shrink command should have 2 arguments");
            }
            _inputs = new List<string>();
            if (arguments.Count == 2)
            {                
                foreach (var parameter in arguments[1].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    AddParameter(parameter);
                }
            }
        }

        public void AddParameter(string parameter)
        {
            _inputs.Add(parameter);
        }

        public void Execute(string rootPath)
        {            
            foreach(var input in _inputs)
            {
                var trimmed = input.Trim();
                var extension = Path.GetExtension(trimmed);
                if (!_compressors.ContainsKey(extension))
                {
                    throw new Exception(string.Format("Extension {0} cannot be shrinked (only .js and .css can)", extension));
                }

                var file = rootPath + trimmed;
                Console.WriteLine("Shrinking {0}", file);
                var contents = File.ReadAllText(file);                
                File.WriteAllText(file, _compressors[extension](contents));                
            }            
        }

    }
}