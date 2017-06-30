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

        public override void Delete(IEnumerable<LogEntity> logs)
        {
            foreach (var log in logs)
            {
                Delete(log);
            }
        }
        public override void Delete(LogEntity log)
        {
            var path = Path.Combine(_WorkFolder,log.CreateTime.ToString("yyyy-MM-dd"), log.Id + ".log");
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

        public override LogEntity[] DequeueBatch(int maxCount = 10000)
        {
            List<LogEntity> result = new List<LogEntity>();
            foreach (var path in Directory.GetFiles(_WorkFolder, "*.log", SearchOption.AllDirectories))
            {
                try
                {
                    var log = File.ReadAllText(path, Encoding.UTF8).Convert2Model<LogEntity>();
                    result.Add(log);
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (result.Count >= maxCount)
                {
                    break;
                }
            }
            return result.ToArray();
        }

        public override LogEntity DequeueSingle()
        {
            LogEntity log = null;
            foreach (var path in Directory.GetFiles(_WorkFolder, "*.log", SearchOption.AllDirectories))
            {
                log = File.ReadAllText(path, Encoding.UTF8).Convert2Model<LogEntity>();
                break;
            }
            return log;
        }

        public override void Enqueue(LogEntity log)
        {
            string folder = Path.Combine(_WorkFolder, log.CreateTime.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string path = Path.Combine(folder, log.Id);
            File.WriteAllText(path + ".temp", log.ToJsonString(), Encoding.UTF8);
            File.Move(path + ".temp", path + ".log");
        }
    }
}
