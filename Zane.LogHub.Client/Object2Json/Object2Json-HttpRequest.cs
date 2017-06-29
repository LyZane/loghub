using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
namespace Zane.LogHub.Client
{
    internal static partial class Object2Json
    {
        internal static object HttpRequest2Json(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            var data = new
            {
                基本信息 = new
                {
                    Url = request.GetDisplayUrl(),
                    HttpMethod = request.Method,
                    IsHttps = request.IsHttps,
                    ContentLength = request.ContentLength
                },
                客户端 = new
                {
                    UserAgent = request.Headers["User-Agent"],
                    UserHostAddress = request.HttpContext.Connection.RemoteIpAddress + "" + request.HttpContext.Connection.RemotePort,
                },
                Form = (from key in request.Form.Keys select new { key = key, value = request.Form[key] }).ToDictionary(a => a.key, a => a.value),
                Cookies = (from key in request.Cookies.Keys select new { key = key, value = request.Cookies[key] }).ToDictionary(a => a.key, a => a.value),
                Files = (from file in request.Form.Files select new { key = file.FileName, value = file.Name + "(" + file.Length + "byte)" }).ToDictionary(a => a.key, a => a.value),
                Headers = (from key in request.Headers.Keys select new { key = key, value = request.Headers[key] }).ToDictionary(a => a.key, a => a.value),
                ServerVariables = new
                {
                    Path = request.Path,
                    PathBase = request.PathBase
                }
            };
            return data;
        }
    }
}
