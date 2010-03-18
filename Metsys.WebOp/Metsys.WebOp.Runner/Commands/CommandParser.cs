namespace Metsys.WebOp.Runner
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class CommandParser
    {
        private static readonly IDictionary<string, Func<string[], ICommand>> _commandCreator = new Dictionary<string, Func<string[], ICommand>>
        {
            {"combine", s => new CombineCommand(s)},
            {"shrink", s => new ShrinkCommand(s)},
            {"busting", s => new BustingCommand(s)},
            {"zip", s => new ZipCommand(s)},
        };
        public static IList<ICommand> Parse(string file)
        {
            var commands = new List<ICommand>();
            using (var sr = new StreamReader(file))
            {
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#")){continue;}
                    
                    var command = ParseLine(line);
                    if (command == null)
                    {
                        Console.WriteLine("Command not understood: " + line);
                    }
                    else
                    {
                        commands.Add(command);    
                    }                    
                }
            }
            return commands;
        }

        private static ICommand ParseLine(string line)
        {
            var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            var command = parts[0].Trim().ToLower();
            return _commandCreator.ContainsKey(command) ? _commandCreator[command](parts) : null;
        }
    }
}