using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Client
{
    public class MemoryStorage : IStorage
    {
        ConcurrentQueue<LogEntity> Logs = new ConcurrentQueue<LogEntity>();
        public void Delete(params string[] ids)
        {
            
        }

        public LogEntity[] DequeueBatch(int maxCount = 100)
        {
            List<LogEntity> result = new List<LogEntity>();
            while (result.Count<maxCount&&Logs.Count>0)
            {
                LogEntity log = null;
                if (Logs.TryDequeue(out log)&&log!=null)
                {
                    result.Add(log);
                }
            }
            return result.ToArray();
        }

        public LogEntity DequeueSingle()
        {
            LogEntity log = null;
            Logs.TryDequeue(out log);
            return log;
        }

        public void Enqueue(LogEntity log)
        {
            Logs.Enqueue(log);
        }
    }
}
