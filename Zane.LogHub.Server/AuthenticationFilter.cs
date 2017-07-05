using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zane.LogHub.Server
{

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    /// <summary>
    /// 对所有请求进行身份验证
    /// </summary>
    public class BasicAuthorizationFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Headers["ApplicationId"].Count==0)
            {
                context.Result = ApiResult.Fail("请在 Header 中声明 ApplicationId。");
                return;
            }
            var authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader.Count == 0)
            {
                context.Result = ApiResult.Fail("请使用 Basic Authorization 进行身份验证。");
                return;
            }
            string authStr = authHeader.ToArray()[0];
            if (authStr.Substring(0, 5).ToLower() != "basic")
            {
                context.Result = ApiResult.Fail("请使用 Basic 方式进行身份验证。");
                return;
            }
            string base64 = authStr.Substring(6);
            string userNameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            int splitIndex = userNameAndPassword.IndexOf(':');
            if (splitIndex < 0 || userNameAndPassword.Length - splitIndex == 1)
            {
                context.Result = ApiResult.Fail("身份验证失败。");
                return;
            }
            string username = userNameAndPassword.Substring(0, splitIndex);
            string password = userNameAndPassword.Substring(splitIndex + 1);
            if (!AppList.Check(username, password))
            {
                context.Result = ApiResult.Fail("身份验证失败。");
                return;
            }
            if (context.HttpContext.Request.Headers["ApplicationId"][0]!= username)
            {
                context.Result = ApiResult.Fail("错误的 ApplicationId。");
                return;
            }
        }
    }
}
