using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zane.LogHub.Client;
using System.Threading;

namespace Zane.LogHub.Client.Test
{
    [TestClass]
    public class LoggerTester
    {
        public LoggerTester()
        {
            GlobalConfiguration.Current
                .SetServerHost(new Uri("http://localhost:19503"))
                .CatchGlobeException()
                .SetStorage(new FileStorage(@"D://LogHubClientWorkFolder"))
                .Startup("TestAppId", "TestAppToken");
        }
        [TestMethod]
        public void TestLog()
        {
            Logger.Log("Unit Test", "hello,this is title", DateTime.Now);
            Thread.Sleep(1000*60);
        }
    }
}
