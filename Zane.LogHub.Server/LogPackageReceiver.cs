using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                        Directory.CreateDirectory(workFolder);
                        singleton = new LogPackageReceiver(workFolder);
                    }
                }
            }
        }
        #endregion

        private static string WorkFolder { get; set; }
        public static void Receive(IFormFile file,string ip)
        {
            using (var stream = new FileStream(Path.Combine(WorkFolder,Path.GetRandomFileName()), FileMode.Create))
            {
                file.CopyToAsync(stream);
            }
        }
    }
}
