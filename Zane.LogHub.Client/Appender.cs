using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Net.Http.Headers;

namespace Zane.LogHub.Client
{
    /// <summary>
    /// 将日志追加到本地库，再从本地库转移到服务器端。
    /// </summary>
    internal class Appender
    {
        #region Singleton
        private static Appender singleton;
        private Appender(IStorage storage)
        {
            this.Storage = storage;
            Worker = new Thread(Processor);
            Worker.Start();
        }
        internal static Appender GetSingleton(IStorage storage)
        {
            if (singleton == null)
            {
                lock (typeof(Appender))
                {
                    if (singleton == null)
                    {
                        singleton = new Appender(storage);
                    }
                }
            }
            return singleton;
        }
        #endregion

        public IStorage Storage { get; set; }
        static Queue<LogEntity> Works = new Queue<LogEntity>();
        private Thread Worker;
        private bool IsRunning = false;



        private void Processor()
        {
            IsRunning = true;
            while (IsRunning)
            {
                if (Works.Count < 1)
                {
                    AddWork();
                }

                LogPackage package = new LogPackage();
                while (Works.Count > 0)
                {
                    package.Add(Works.Dequeue());
                    if (package.ContentLength >= 1024 * 1024 * 5)
                    {
                        break;
                    }
                }

                // 将日志发往服务器端
                if (Send(package))
                {
                    // 发送成功后在本地 Storage 中删除日志
                    this.Storage.Delete(package.Select(a => a.Item1));
                }
                else
                {
                    // 发送失败后将日志追加到发送队列末端，等待重试。
                    foreach (var item in package)
                    {
                        Works.Enqueue(item.Item1);
                    }
                }


            }
        }


        private bool Send(LogPackage package)
        {
            // 将 package 中的所有日志压缩成一个 zip 对象，并存放在 MemoryStream 中。
            MemoryStream packageStream = new MemoryStream(package.ContentLength);
            using (ZipArchive zipArchive = new ZipArchive(packageStream, ZipArchiveMode.Create))
            {
                foreach (var item in package)
                {
                    ZipArchiveEntry entity = zipArchive.CreateEntry(item.Item1.Id + ".txt");
                    using (StreamWriter writer = new StreamWriter(entity.Open()))
                    {
                        writer.Write(item.Item2);
                    }
                }
            }

            
            // 将 zip 对象所在的 MemoryStream 以文件的形式发送到服务器端。
            using (var formDataContent = new MultipartFormDataContent())
            {
                var zipData = packageStream.ToArray();
                // 添加 zip 对象的 MD5 值
                formDataContent.Headers.ContentMD5 = MD5.Create().ComputeHash(zipData);
                // 以文件的形式上传 zip 对象
                formDataContent.Add(new ByteArrayContent(zipData), "files", "LogPackage.zip");
                
                using (HttpClient httpClient = new HttpClient() { DefaultRequestHeaders = { Authorization = GlobalConfiguration.Current.HttpAuthValue } })
                {
                    HttpResponseMessage response = httpClient.PostAsync(GlobalConfiguration.Current.ServerUrl_Upload, formDataContent).Result;
                }
            }

            using (packageStream) { }
            return true;
        }


        internal void AddWork()
        {
            if (Works.Count > 1)
            {
                return;
            }
            LogEntity[] list = this.Storage.DequeueBatch();
            foreach (var item in list)
            {
                Works.Enqueue(item);
            }

            if (list.Length < 1)
            {
                Thread.Sleep(10000); //休息10秒钟
                AddWork();
            }
        }
        internal void Save(LogEntity log)
        {
            Task.Run(() => this.Storage.Enqueue(log));
        }

    }
}
