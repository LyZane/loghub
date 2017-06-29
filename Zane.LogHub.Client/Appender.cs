using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zane.LogHub.Client
{
    /// <summary>
    /// 将日志追加到本地库，再从本地库转移到服务器端。
    /// </summary>
    internal class Appender
    {
        #region Singleton
        private static Appender singleton;
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

        private IStorage Storage { get; set; }
        static Queue<LogEntity> Works = new Queue<LogEntity>();
        private Thread Worker;
        private bool IsRunning = false;
        
        private Appender(IStorage storage)
        {
            // 检查 Storage 读、写、删 是否正常。
            try
            {
                storage.Enqueue(new LogEntity("InitTest", "This is a test log, used to detect whether the storage object is ok."));
                storage.Delete(storage.DequeueSingle().Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Storage 不可用，请检查 Storage 的 读、写、删 功能是否正常。");
            }
            Worker = new Thread(Processor);
            Worker.Start();
        }

        private void Processor()
        {
            IsRunning = true;
            while (IsRunning)
            {
                if (Works.Count < 1) { AddWork();continue; }
                List<Tuple<LogEntity, string>> package = new List<Tuple<LogEntity, string>>();
                int length = 0;
                while (Works.Count > 0)
                {
                    LogEntity log = Works.Dequeue();
                    var jsonStr = log.ToJsonString();
                    length += jsonStr.Length;

                    if (length >= 1024 * 1024 * 5)
                    {
                        break;
                    }
                }

                FileStream packageStream = new FileStream("TestPackage.zip", FileMode.OpenOrCreate);
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

                using (packageStream){}

                //var archiveEntity = new ZipArchiveEntry();
                

                //bool result = false;
                //for (int i = 0; i < 3; i++) //请求3次
                //{
                //    result = PostRequest(Setting.API_URL_Log, fileInfo.FullName, Guid.NewGuid().ToString());
                //    if (result)
                //    {
                //        break;
                //    }
                //}

                //if (!result)
                //{
                //    works.Enqueue(fileInfo.FullName);
                //}
                //else
                //{
                //    DeleteFile(fileInfo);
                //}
            }
        }


        //private static bool PostRequest(string url, string fileNamePath, string saveName)
        //{
        //    FileStream fs;
        //    BinaryReader r;
        //    try
        //    {
        //        fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read, FileShare.None);
        //        r = new BinaryReader(fs);
        //    }
        //    catch (Exception)
        //    {
        //        if (!File.Exists(fileNamePath))
        //        {
        //            return true;
        //        }
        //        return false;
        //    }


        //    //时间戳 
        //    string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
        //    byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");

        //    //请求头部信息 
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("--");
        //    sb.Append(strBoundary);
        //    sb.Append("\r\n");
        //    sb.Append("Content-Disposition: form-data; name=\"");
        //    sb.Append("file");
        //    sb.Append("\"; filename=\"");
        //    sb.Append(saveName);
        //    sb.Append("\"");
        //    sb.Append("\r\n");
        //    sb.Append("Content-Type: ");
        //    sb.Append("application/octet-stream");
        //    sb.Append("\r\n");
        //    sb.Append("\r\n");
        //    string strPostHeader = sb.ToString();
        //    byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

        //    // 根据uri创建HttpWebRequest对象 
        //    HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
        //    httpReq.Method = "POST";

        //    //对发送的数据不使用缓存 
        //    httpReq.AllowWriteStreamBuffering = false;

        //    //设置获得响应的超时时间（30秒） 
        //    httpReq.Timeout = 30000;
        //    httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
        //    long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
        //    long fileLength = fs.Length;
        //    httpReq.ContentLength = length;
        //    try
        //    {
        //        //每次上传4k 
        //        int bufferLength = 4096;
        //        byte[] buffer = new byte[bufferLength];

        //        //已上传的字节数 
        //        long offset = 0;

        //        //开始上传时间 
        //        DateTime startTime = DateTime.Now;
        //        int size = r.Read(buffer, 0, bufferLength);
        //        Stream postStream = httpReq.GetRequestStream();

        //        //发送请求头部消息 
        //        postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
        //        while (size > 0)
        //        {
        //            postStream.Write(buffer, 0, size);
        //            offset += size;
        //            size = r.Read(buffer, 0, bufferLength);
        //        }
        //        //添加尾部的时间戳 
        //        postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
        //        postStream.Close();

        //        //获取服务器端的响应
        //        WebResponse webRespon = httpReq.GetResponse();
        //        Stream s = webRespon.GetResponseStream();
        //        StreamReader sr = new StreamReader(s);

        //        //读取服务器端返回的消息 
        //        String sReturnString = sr.ReadLine();
        //        if (sReturnString != "ok")
        //        {
        //            throw new Exception();
        //        }
        //        s.Close();
        //        sr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        fs.Close();
        //        r.Close();
        //    }
        //    return true;
        //}
        internal void AddWork()
        {
            if (Works.Count >1)
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
            Task.Run(()=>this.Storage.Enqueue(log));
        }
         
    }
}
