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
                        Directory.CreateDirectory(workFolder);
                        singleton = new LogPackageReceiver(workFolder);
                    }
                }
            }
        }
        #endregion

        private static string WorkFolder { get; set; }
        public static void Receive(IFormFile formFile,string ip)
        {
            string path = Path.Combine(WorkFolder, Path.GetRandomFileName());
            using (var stream = new FileStream(path + ".temp", FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
            File.Move(path + ".temp", path + ".zip");
        }
    }
}
