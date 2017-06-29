using System;
using System.IO;
using System.Linq;

namespace Zane.LogHub.Client
{
    public static class Logger
    {
        private static Appender _Appender = Appender.GetSingleton(new FileStorage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogRepository")));
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
