using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
        public static void Receive(IFormFile formFile,StringValues md5Headers,string appId,string ip)
        {
            // 对文件进行 MD5 校验
            if (md5Headers.Count == 0)
            {
                throw new Exception("客户端请求缺少必要的 Header 信息：Content-MD5。");
            }
            using (MemoryStream stream = new MemoryStream(Convert.ToInt32(formFile.Length)))
            {
                formFile.CopyTo(stream);
                if (md5Headers[0] != Convert.ToBase64String(MD5.Create().ComputeHash(stream.ToArray())))
                {
                    throw new Exception($"上传的文件 MD5 校对失败，服务器端收到的文件长度为{formFile.Length}byte，请核对文件是否被修改。");
                }
            }

            string path = LogPackageProcessor.CreateLogPackageFileName(appId, ip);
            using (var stream = new FileStream(path + ".temp", FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
            File.Move(path + ".temp", path + ".zip");
        }
    }
}
