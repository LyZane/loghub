using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Zane.LogHub.Client
{
    public class Configuration
    {
        #region API Url
        private Uri DefaultServiceHost = new Uri("http://LogHub.in");
        public Uri ServiceHost { get { return DefaultServiceHost; } internal set { DefaultServiceHost = value; } }
        //接口地址：日志上传
        internal string ServerUrl_Upload
        {
            get { return new Uri(DefaultServiceHost, "api/log").AbsoluteUri; }
        }
        #endregion
        
        #region Auth
        public string ApplicationId { get;internal set; }
        public string ApplicationToken { get; internal set; }
        private AuthenticationHeaderValue _HttpAuthValue;
        /// <summary>
        /// 将日志发送到服务器端时所使用的 Auth 信息
        /// </summary>
        public AuthenticationHeaderValue HttpAuthValue
        {
            get
            {
                if (_HttpAuthValue == null)
                {
                    _HttpAuthValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApplicationId}:{ApplicationToken}")));
                }
                return _HttpAuthValue;
            }
        } 
        #endregion
        
    }
    public static class GlobalConfiguration
    {
        public readonly static Configuration Current = new Configuration();
        /// <summary>
        /// 组件是否已初始化
        /// </summary>
        public static bool Initialized { get; private set; }
        /// <summary>
        /// 检查当前的配置项，并启动日志服务。
        /// </summary>
        /// <param name="config"></param>
        public static void Startup(this Configuration config,string applicationId,string applicationToken)
        {
            if (Initialized)
            {
                throw new MethodAccessException("Zane.LogHub.Client is Started.");
            }
            if (Logger.Singleton==null)
            {
                throw new MethodAccessException("Must be after SetStorage.");
            }
            Current.ApplicationId = applicationId;
            Current.ApplicationToken = applicationToken;
        }
        public static Configuration SetStorage(this Configuration config, IStorage storage)
        {
            if (Initialized)
            {
                throw new MethodAccessException("Must be before Startup.");
            }
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }
            Logger.CreateSingleton(storage);
            return Current;
        }
        public static Configuration SetServerHost(this Configuration config, Uri host, bool check = false)
        {
            if (Initialized)
            {
                throw new MethodAccessException("Must be before Startup.");
            }
            if (check)
            {
                var client = new HttpClient();
                var t = client.GetStringAsync(host);
                t.Wait();
                string str = t.Result;
            }
            Current.ServiceHost = host;
            return Current;
        }
        
        public static Configuration CatchGlobeException(this Configuration config)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;
            return Current;
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
