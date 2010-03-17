namespace Metsys.WebOp.Mvc
{
    using System;

    public class WebOpConfiguration
    {
        private WebOpConfiguration(){}
        
        public static void Initialize(Action<IConfiguration> action)
        {
            action(Configuration.Instance);
        }
    }
}