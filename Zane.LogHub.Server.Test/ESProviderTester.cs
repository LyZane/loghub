using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zane.LogHub.Server.Test
{
    [TestClass]
    public class ESProviderTester
    {
        [TestMethod]
        public void TestListTags()
        {
            var result = ESProvider.ListTags("TestApp");
            Console.WriteLine(result);
        }

        [TestMethod]
        public void TestCreateIndex()
        {
            var result = ESProvider.CreateIndex("TestApp");
            Console.WriteLine(result);
        }
        
    }
}
