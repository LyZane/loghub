﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Server
{
    internal static class AppList
    {
        private static readonly Dictionary<string, AppItem> Apps = new Dictionary<string, AppItem>();
        static AppList()
        {
            
            AppItem app = new AppItem("TestApp","TestToken");
            Add(app);
        }
        internal static void Add(AppItem app)
        {
            Apps.Add(app.Id,app);
        }
        
        internal static bool Check(string appId,string appToken)
        {
            if (Apps.ContainsKey(appId))
            {
                if (Apps[appId].Token == appToken)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
