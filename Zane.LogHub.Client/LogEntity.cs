using System;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Client
{
    public class LogEntity
    {
        public LogEntity(string tag, string title, params ContentEntity[] contents)
        {
            this.Id = Setting.ApplicationId + "--" + Guid.NewGuid();
            this.Tag = tag;
            this.Title = title;
            this.Contents = contents;
            this.CreateTime = DateTime.Now;
            this.Timestamp = this.CreateTime.ToUnixTime();
            this.ApplicationId = Setting.ApplicationId;
            this.UDID = BaseProperty.UDID;
            this.DeviceModel = BaseProperty.DeviceModel;
            this.DeviceNickName = BaseProperty.DeviceNickName;
            this.OsVersion_Str = BaseProperty.OsVersion_Str;
            this.OsVersion_Num = BaseProperty.OsVersion_Num;
            this.AppVersion = BaseProperty.AppVersion;
        }
        public string Id { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public ContentEntity[] Contents { get; set; }
        /// <summary>
        /// 日志的创建时间(时间戳，精确到毫秒)
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 日志的创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        public string ApplicationId { get; set; }

        public string UDID { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceNickName { get; set; }
        public string OsVersion_Str { get; set; }
        public string OsVersion_Num { get; set; }
        public string AppVersion { get; set; }
    }
}
