namespace Metsys.WebOp.Runner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    
    public class ZipCommand: ICommand
    {        
        private readonly IList<string> _inputs;

        public ZipCommand(IList<string> arguments)
        {
            if (arguments.Count < 1)
            {
                throw new Exception("Zip command should have 2 arguments");
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
                Compress(rootPath + input.Trim());
            }            
        }
        
        private static void Compress(string file)
        {
            // Get the stream of the source file.
            using (var input = File.OpenRead(file))
            using (var output = File.Create(file + ".gz"))
            using (var compress = new GZipStream(output, CompressionMode.Compress))
            {
                var buffer = new byte[8192];
                int numRead;
                while ((numRead = input.Read(buffer, 0, buffer.Length)) != 0)
                {
                    compress.Write(buffer, 0, numRead);
                }                
            }     
        }
    }
}