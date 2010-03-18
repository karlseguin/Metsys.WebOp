namespace Metsys.WebOp.Runner
{
    using System;
    using System.IO;
    using System.IO.Compression;
    
    public class ZipCommand: ICommand
    {        
        private readonly string[] _inputs;

        public ZipCommand(string[] arguments)
        {
            if (arguments.Length != 2)
            {
                throw new Exception("Zip command should have 2 arguments");
            }
            _inputs = arguments[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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