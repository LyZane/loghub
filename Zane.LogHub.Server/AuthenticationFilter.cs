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
    public class BasicAuthorizationFilter :Attribute, IAuthorizationFilter //, IAuthenticationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
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

        }
    }
}
