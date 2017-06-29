using System;

namespace Zane.LogHub.Client.CmdDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration.CatchGlobeException().SetStorage(new FileStorage(@"D://LogHubWorkFolder")).Startup();
            Logger.Log("Unit Test", "hello,this is title", DateTime.Now);
            Console.ReadLine();
        }
    }
}