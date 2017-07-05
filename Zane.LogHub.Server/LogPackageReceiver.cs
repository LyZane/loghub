using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Zane.LogHub.Server
{
    public class LogPackageReceiver
    {
        #region Singleton
        private static LogPackageReceiver singleton;
        private LogPackageReceiver(string workFolder)
        {
            WorkFolder = workFolder;
        }
        public static void CreateSingleton(string workFolder)
        {
            if (singleton == null)
            {
                lock (typeof(LogPackageReceiver))
                {
                    if (singleton == null)
                    {
                        // TODO: 检查文件夹的 读、写、删 权限
                        Directory.CreateDirectory(workFolder);
                        singleton = new LogPackageReceiver(workFolder);
                        LogPackageProcessor.Start(workFolder);
                    }
                }
            }
        }
        #endregion

        private static string WorkFolder { get; set; }
        public static void Receive(IFormFile formFile,string appId,string ip)
        {
            string path = Path.Combine(WorkFolder, $"{appId}--{ip.Replace(":","0.")}--{Path.GetRandomFileName()}");
            using (var stream = new FileStream(path + ".temp", FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
            File.Move(path + ".temp", path + ".zip");
        }
    }
}
