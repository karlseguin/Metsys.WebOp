namespace Metsys.WebOp.Runner
{
    using System;    
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class CombineCommand : ICommand
    {
        private readonly string _output;
        private readonly IList<string> _inputs;
        
        public CombineCommand(IList<string> arguments)
        {
            if (arguments.Count < 2)
            {
                throw new Exception("Combine command should have 2 arguments");
            }
            _output = arguments[1];
            _inputs = new List<string>();

            if (arguments.Count == 3)
            {                
                foreach (var parameter in arguments[2].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
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
            Console.WriteLine("Combining {0} to {1}", string.Join(",", _inputs.ToArray()), _output);
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