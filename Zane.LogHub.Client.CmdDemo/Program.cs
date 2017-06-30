using System;
using System.Threading;

namespace Zane.LogHub.Client.CmdDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Current
                .SetServerHost(new Uri("http://localhost:19503"))
                .CatchGlobeException()
                .SetStorage(new FileStorage(@"D://LogHubClientWorkFolder"))
                .Startup("TestAppId", "TestAppToken");

            while (true)
            {
                Logger.Log("Unit Test", DateTime.Now.ToString(), DateTime.Now);
                Thread.Sleep(1000 * 5);
            }

            Console.ReadLine();
        }
    }
}