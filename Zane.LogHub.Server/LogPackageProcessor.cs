using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Zane.LogHub.Server
{
    internal static class LogPackageProcessor
    {
        private static bool IsRunning = false;
        private static string WorkFolder { get; set; }
        private static Thread Worker;
        /// <summary>
        /// 初始化，并启动 LogPackage 处理器。
        /// </summary>
        /// <param name="workFolder"></param>
        internal static void Start(string workFolder)
        {
            if (IsRunning)
            {
                return;
            }
            else
            {
                lock (typeof(LogPackageProcessor))
                {
                    if (IsRunning)
                    {
                        return;
                    }
                    else
                    {
                        IsRunning = true;
                        WorkFolder = workFolder;
                        Worker = new Thread(Process);
                        Worker.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 处理器启动后会持续工作
        /// </summary>
        private static void Process()
        {
            if (!IsRunning)
            {
                throw new MethodAccessException("Must be after Init.");
            }

            while (true)
            {

                var files = Directory.GetFiles(WorkFolder, "*.zip", SearchOption.TopDirectoryOnly);
                if (files.Length >0)
                {
                    Parallel.ForEach(files,new ParallelOptions() { MaxDegreeOfParallelism=1 }, path => {
                        Core(path);
                    });
                }
                else
                {
                    Thread.Sleep(1000 * 5);
                }
            }
        }

        /// <summary>
        /// 单个 LogPackage 的处理流程
        /// </summary>
        /// <param name="path"></param>
        private static void Core(string path)
        {
            var package = new FileInfo(path);
            if (!package.Exists)
            {
                return;
            }
            // 修改文件名，标记此 LogPackage 处于被处理中。
            File.Move(path, path + ".processing");
            
            var temp = ResolveFileName(package.Name);
            string extractFolder = Path.Combine(WorkFolder, temp.appId, temp.ip);

            if (!Directory.Exists(extractFolder))
            {
                Directory.CreateDirectory(extractFolder);
            }
            ZipFile.ExtractToDirectory(path + ".processing", Path.Combine(WorkFolder, extractFolder));
            File.Delete(path + ".processing");
        }

        /// <summary>
        /// 从文件名中提取出 appId 和 IP。
        /// 文件名格式：{appId}--{ip}--{Path.GetRandomFileName()}.xxx
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static (string appId, string ip) ResolveFileName(string fileName)
        {
            var temp = fileName.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length != 3)
            {
                throw new Exception("LogPackage未按约定设置文件名。");
            }
            string appId = temp[0];
            string ip = temp[1];
            return (appId, ip);
        }

        internal static string CreateLogPackageFileName(string appId,string ip)
        {
            return Path.Combine(WorkFolder, $"{appId}--{ip.Replace(":", "0.")}--{Path.GetRandomFileName()}");
        }
    }
}
