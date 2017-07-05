using System;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Server
{
    public class AppItem
    {
        public AppItem(string appId,string appToken)
        {
            this.Id = appId;
            this.Token = appToken;
        }
        public string Id { get; set; }
        public string Token { get; set; }
    }
}
