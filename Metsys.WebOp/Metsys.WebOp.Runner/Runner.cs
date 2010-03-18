namespace Metsys.WebOp.Runner
{
    using System;
    using System.IO;

    public class Runner
    {
        private readonly string _root;
        private readonly string _commandFile;
        private const string _commandFileName = "webop.dat";
        
        
        public Runner(string root)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                throw new ArgumentException(string.Format("{0} is not a valid directory", root));
            }
            _root = root.EndsWith("\\") ? root : string.Concat(root, '\\');
                        
            _commandFile = string.Concat(_root, _commandFileName);
            if (!File.Exists(_commandFile))
            {
                throw new ArgumentException(string.Format("{0} could not be found", _commandFile));
            }            
        }
        
        public void Run()
        {
            foreach(var command in CommandParser.Parse(_commandFile))
            {
                command.Execute(_root);
            }            
        }
    }
}