using System;
using System.Text;
using System.Threading;

namespace Zane.LogHub.Client.CmdDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            GlobalConfiguration.Current
                .SetServerHost(new Uri("http://localhost:19503"))
                .CatchGlobeException()
                .SetStorage(new FileStorage(@"D://LogHubClientWorkFolder"))
                .Startup("TestAppId", "TestAppToken");

            //Logger.Log("Unit Test", DateTime.Now.ToString(), DateTime.Now);
            //Console.WriteLine("loged");
            //return;

            while (true)
            {
                Logger.Log("Unit Test", DateTime.Now.ToString(), DateTime.Now);
                Thread.Sleep(10);
            }

            Console.ReadLine();
        }
    }
}