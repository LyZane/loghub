using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Zane.LogHub.Client
{
    public class Configuration
    {
        private Uri DefaultCloudHost = new Uri("http://Exception.cloud");
        public Uri CloudHost { get { return DefaultCloudHost; } internal set { DefaultCloudHost = value; } }
    }
    public static class GlobalConfiguration
    {
        public readonly static Configuration Configuration = new Configuration();
        /// <summary>
        /// 组件是否已初始化
        /// </summary>
        public static bool Initialized { get; private set; }
        /// <summary>
        /// 检查当前的配置项，并启动日志服务。
        /// </summary>
        /// <param name="config"></param>
        public static void Startup(this Configuration config)
        {
            if (Initialized)
            {
                throw new MethodAccessException("Zane.LogHub.Client is Initialized.");
            }
        }
        public static Configuration SetStorage(this Configuration config, IStorage storage)
        {
            if (Initialized)
            {
                throw new MethodAccessException("Must be before initialization.");
            }
            if (storage==null)
            {
                throw new ArgumentNullException(nameof(storage));
            }
            storage.Test();
            Logger.GetSingleton(storage);
            return config;
        }
        public static Configuration SetCloudHost(this Configuration config, Uri host, bool check = false)
        {
            if (Initialized)
            {
                throw new MethodAccessException("Must be before initialization.");
            }
            if (check)
            {
                var client = new HttpClient();
                var t = client.GetStringAsync(host);
                t.Wait();
                string str = t.Result;
            }
            config.CloudHost = host;
            return config;
        }
        
        public static Configuration CatchGlobeException(this Configuration config)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;
            return config;
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (e.ExceptionObject as Exception);
            Logger.Log("UnhandledException", ex.Message,ex);
        }

        private static void UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Logger.Log("UnobservedTaskException", ex.Message, ex);
        }
    }
}
