using System;
using System.IO;
using System.Linq;

namespace Zane.LogHub.Client
{
    public class Logger
    {
        #region Singleton
        private static Logger singleton;
        private Logger(IStorage storage)
        {
            _Appender = Appender.GetSingleton(storage);
        }
        internal static Logger GetSingleton(IStorage storage)
        {
            if (singleton == null)
            {
                lock (typeof(Logger))
                {
                    if (singleton == null)
                    {
                        singleton = new Logger(storage);
                    }
                }
            }
            return singleton;
        }
        #endregion

        private static Appender _Appender;
        public static void Log(string tag, string title, params object[] contents)
        {
            Log(tag, title, contents.Select(a => new ContentEntity(a)).ToArray());
        }
        public static void Log(string tag, string title, params ContentEntity[] contents)
        {
            _Appender.Save(new LogEntity(tag,title,contents));
        }
    }
}
