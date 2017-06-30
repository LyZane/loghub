using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Zane.LogHub.Client
{
    public static class Extensions
    {
        /// <summary>
        /// 计算 byte[] 的MD5值。
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string MD5Encrypt(this byte[] arr)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(arr);
                return Encoding.ASCII.GetString(result);
            }
        }

        /// <summary>
        /// 把DateTime转成时间戳
        /// 精确到毫秒级别
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime dateTime)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (dateTime.Ticks - dt1970.Ticks) / 10000;
        }

        /// <summary>
        /// 把时间戳转换成DateTime
        /// 精确到毫秒级别
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timestamp)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long t = dt1970.Ticks + timestamp * 10000;
            return new DateTime(t);
        }

        /// <summary>
        /// 将此对象序列化成json字符串。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj)
        {
            StringBuilder sb = new StringBuilder();
            JsonSerializer serializer = new JsonSerializer()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            serializer.Converters.Add(new MemoryStreamJsonConverter());
            //捕获序列化时产生的异常，并不对异常做处理。
            serializer.Error += delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };
            StringWriter sw = new StringWriter(sb);
            serializer.Serialize(sw, obj);
            return sb.ToString();
        }

        /// <summary>
        /// 将Json字符串转换为指定类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T Convert2Model<T>(this string jsonStr) where T : class
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

    }
    internal class MemoryStreamJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MemoryStream).GetTypeInfo().IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var bytes = serializer.Deserialize<byte[]>(reader);
            return bytes != null ? new MemoryStream(bytes) : new MemoryStream();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bytes = ((MemoryStream)value).ToArray();
            serializer.Serialize(writer, bytes);
        }
    }
}
