namespace Metsys.WebOp.Runner
{
    using System;
    using System.IO;

    public class CombineCommand : ICommand
    {
        private readonly string _output;
        private readonly string[] _inputs;
        
        public CombineCommand(string[] arguments)
        {
            if (arguments.Length != 3)
            {
                throw new Exception("Combine command should have 3 arguments");
            }
            _output = arguments[1];
            _inputs = arguments[2].Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);            
        }

        public void Execute(string rootPath)
        {
            Console.WriteLine("Combining {0} to {1}", string.Join(",", _inputs), _output);
            using (var sw = new StreamWriter(rootPath + _output.Trim(), false))
            {
                foreach(var input in _inputs)
                {
                    sw.WriteLine(File.ReadAllText(rootPath + input.Trim()));
                }
            }
        }
    }
}