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
                .Startup("TestApp", "TestToken");

            //Logger.Log("Unit Test", DateTime.Now.ToString(), DateTime.Now);
            //Console.WriteLine("loged");
            //return;
            Random ran = new Random();
            while (true)
            {
                //Console.WriteLine("按任意键写入一条日志：");
                //Console.ReadKey();
                Logger.Log("Unit Test"+ran.Next(100,999), DateTime.Now.ToString(), AppDomain.CurrentDomain);
                Thread.Sleep(1);
            }

            Console.ReadLine();
        }
    }
}