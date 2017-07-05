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
    internal class LogManager
    {
        #region Singleton
        private static LogManager singleton;
        private LogManager(IStorage storage)
        {
            this.Storage = storage;
            Worker = new Thread(Processor);
            Worker.Start();
        }
        internal static LogManager GetSingleton(IStorage storage)
        {
            if (singleton == null)
            {
                lock (typeof(LogManager))
                {
                    if (singleton == null)
                    {
                        singleton = new LogManager(storage);
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
                    // 发送失败通常是网络原因，等待一分钟后继续。
                    Thread.Sleep(1000*60);
                }


            }
        }


        private bool Send(LogPackage package)
        {

            MemoryStream packageStream = new MemoryStream(package.ContentLength);
            // 将 package 中的所有日志压缩成一个 zip 对象，并存放在 MemoryStream 中。
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

            var zipData = packageStream.ToArray();
            packageStream.Dispose();

            // 将 zipData 以文件的形式发送到服务器端。
            //var formDataContent = new MultipartFormDataContent();
            //
            //formDataContent.Headers.ContentLength = zipData.Length;

            //// 以文件的形式上传 zip 对象
            //var fileContent = new ByteArrayContent(zipData);
            //fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "LogPackage.zip" };
            //formDataContent.Add(fileContent);

            using (HttpClient httpClient = new HttpClient() { DefaultRequestHeaders = { Authorization = GlobalConfiguration.Current.HttpAuthValue } })
            {
                MultipartFormDataContent formDataContent = new MultipartFormDataContent();
                // 添加 zip 对象的 MD5 值
                formDataContent.Headers.ContentMD5 = MD5.Create().ComputeHash(zipData);
                formDataContent.Headers.Add("ApplicationId", GlobalConfiguration.Current.ApplicationId);
                ByteArrayContent bytes = new ByteArrayContent(zipData);
                formDataContent.Add(bytes, "file", "LogPackage.zip");

                HttpResponseMessage response = httpClient.PostAsync(GlobalConfiguration.Current.ServerUrl_Upload, formDataContent).Result;
                Console.WriteLine($"上传{package.Count()}条日志完毕，结果：{response.IsSuccessStatusCode}");
                return response.IsSuccessStatusCode;
            }


        }


        internal void AddWork()
        {
            if (Works.Count > 1)
            {
                return;
            }
            LogEntity[] list = this.Storage.DequeueBatch(10000);
            foreach (var item in list)
            {
                Works.Enqueue(item);
            }

            if (list.Length < 1)
            {
                Thread.Sleep(1000 * 10); //休息10秒钟
                AddWork();
            }
        }
        internal void Save(LogEntity log)
        {
            Task.Run(() => this.Storage.Enqueue(log));
        }

    }
}
