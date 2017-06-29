using System;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Client
{
    public static class Setting
    {
        public static string ApplicationId { get; set; }
        public static string ApplicationToken { get; set; }
        //接口地址：根
        internal static Uri API_URL = new Uri("http://log.beyebe.cn/");
        //接口地址：日志写入
        internal static string API_URL_Log
        {
            get { return new Uri(Setting.API_URL, "API/input.ashx").OriginalString; }
        }
    }
}
