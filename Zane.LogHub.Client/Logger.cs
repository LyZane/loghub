using System;
using System.IO;
using System.Linq;

namespace Zane.LogHub.Client
{
    public class Logger
    {
        #region Singleton
        public static Logger Singleton;
        private Logger(IStorage storage)
        {
            _Appender = LogManager.GetSingleton(storage);
        }
        internal static void CreateSingleton(IStorage storage)
        {
            if (Singleton == null)
            {
                lock (typeof(Logger))
                {
                    if (Singleton == null)
                    {
                        if (storage.Test())
                        {
                            Singleton = new Logger(storage);
                        }
                    }
                }
            }
        }
        #endregion

        private static LogManager _Appender;
        public static void Log(string tag, string title, params object[] contents)
        {
            Log(tag, title, contents.Select(a => new ContentEntity(a)).ToArray());
        }
        public static void Log(string tag, string title, params ContentEntity[] contents)
        {
            if (Singleton==null)
            {
                throw new Exception("Must be after initialization Logger.");
            }
            _Appender.Save(new LogEntity(tag,title,contents));
        }
    }
}
