using System;
using System.Collections.Generic;
using System.Text;

namespace Zane.LogHub.Client
{
    internal static class BaseProperty
    {
        /// <summary>
        /// 设备标识
        /// </summary>
        internal static string UDID = System.Environment.MachineName;
        /// <summary>
        /// 1. 用于记录移动设备的品牌与型号
        /// 2. 用于记录web程序的浏览器名称
        /// </summary>
        internal static string DeviceModel { get; set; }
        /// <summary>
        /// 设备自定义名称（别名）
        /// </summary>
        internal static string DeviceNickName { get; set; }
        /// <summary>
        /// 操作系统版本号（字符形式）。也可用于记录web程序的浏览器版本号。
        /// </summary>
        internal static string OsVersion_Str { get; set; }
        /// <summary>
        /// 操作系统版本号（数字形式）。也可用于记录web程序的浏览器版本号。
        /// </summary>
        internal static string OsVersion_Num { get; set; }
        /// <summary>
        /// 应用程序版本
        /// </summary>
        internal static string AppVersion { get; set; }
    }
}
