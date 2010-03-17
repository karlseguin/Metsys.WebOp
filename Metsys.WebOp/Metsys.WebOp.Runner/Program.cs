namespace Metsys.WebOp.Runner
{
    using System;
    
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new InvalidOperationException("Runner requires two parameter (root path, and command filename)");
            }
            new Runner(args[0]).Run();
        }
    }
}
