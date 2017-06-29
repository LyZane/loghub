using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Zane.LogHub.Client
{
    public class FileStorage : IStorage
    {
        private string _WorkFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogStorage");
        public FileStorage(string workFolder = null)
        {
            if (!string.IsNullOrEmpty(workFolder))
            {
                _WorkFolder = workFolder;
            }
            if (!Directory.Exists(_WorkFolder))
            {
                Directory.CreateDirectory(_WorkFolder);
            }
        }

        public void Delete(params string[] ids)
        {
            foreach (var id in ids)
            {
                var path = Path.Combine(_WorkFolder, id + ".log");
                while (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        public LogEntity[] DequeueBatch(int maxCount = 100)
        {
            List<LogEntity> result = new List<LogEntity>();
            foreach (var path in Directory.GetFiles(_WorkFolder, "*.log", SearchOption.AllDirectories))
            {
                var log = File.ReadAllText(path, Encoding.UTF8).Convert2Model<LogEntity>();
                result.Add(log);
                if (result.Count >= maxCount)
                {
                    break;
                }
            }
            return result.ToArray();
        }

        public LogEntity DequeueSingle()
        {
            LogEntity log = null;
            foreach (var path in Directory.GetFiles(_WorkFolder, "*.log"))
            {
                log = File.ReadAllText(path, Encoding.UTF8).Convert2Model<LogEntity>();
                break;
            }
            return log;
        }

        public void Enqueue(LogEntity log)
        {
            File.WriteAllText(Path.Combine(_WorkFolder, log.CreateTime.Day.ToString(), log.Id + ".log"), log.ToJsonString(), Encoding.UTF8);
        }
    }
}
